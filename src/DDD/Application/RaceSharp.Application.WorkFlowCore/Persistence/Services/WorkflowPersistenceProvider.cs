using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkflowCore.Exceptions;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace RaceSharp.Application.WorkFlowCore.Persistence
{
	public class WorkflowPersistenceProvider : IPersistenceProvider
	{
		private readonly IMongoDatabase _database;

		public WorkflowPersistenceProvider(IMongoDatabase database)
		{
			_database = database;
			CreateIndexes(this);
		}

		static WorkflowPersistenceProvider()
		{
			BsonClassMap.RegisterClassMap<WorkflowInstance>(x =>
			{
				x.MapIdProperty(y => y.Id)
					.SetIdGenerator(new StringObjectIdGenerator());
				x.MapProperty(y => y.Data);
				//.SetSerializer(new DataObjectSerializer());
				x.MapProperty(y => y.Description);
				x.MapProperty(y => y.Reference);
				x.MapProperty(y => y.WorkflowDefinitionId);
				x.MapProperty(y => y.Version);
				x.MapProperty(y => y.NextExecution);
				x.MapProperty(y => y.Status);
				x.MapProperty(y => y.CreateTime);
				x.MapProperty(y => y.CompleteTime);
				x.MapProperty(y => y.ExecutionPointers);
			});

			BsonClassMap.RegisterClassMap<EventSubscription>(x =>
			{
				x.MapIdProperty(y => y.Id)
					.SetIdGenerator(new StringObjectIdGenerator());
				x.MapProperty(y => y.EventName);
				x.MapProperty(y => y.EventKey);
				x.MapProperty(y => y.StepId);
				x.MapProperty(y => y.WorkflowId);
				x.MapProperty(y => y.SubscribeAsOf);
				x.MapProperty(y => y.SubscriptionData);
				x.MapProperty(y => y.ExternalToken);
				x.MapProperty(y => y.ExternalWorkerId);
				x.MapProperty(y => y.ExternalTokenExpiry);
				x.MapProperty(y => y.ExecutionPointerId);
			});

			BsonClassMap.RegisterClassMap<Event>(x =>
			{
				x.MapIdProperty(y => y.Id)
					.SetIdGenerator(new StringObjectIdGenerator());
				x.MapProperty(y => y.EventName);
				x.MapProperty(y => y.EventKey);
				x.MapProperty(y => y.EventData);
				x.MapProperty(y => y.EventTime);
				x.MapProperty(y => y.IsProcessed);
			});

			BsonClassMap.RegisterClassMap<ControlPersistenceData>(x => x.AutoMap());
			BsonClassMap.RegisterClassMap<SchedulePersistenceData>(x => x.AutoMap());
			BsonClassMap.RegisterClassMap<ExecutionPointer>(x => x.AutoMap());
			BsonClassMap.RegisterClassMap<ActivityResult>(x => x.AutoMap());
		}

		private static bool indexesCreated = false;

		private static void CreateIndexes(WorkflowPersistenceProvider instance)
		{
			if (!indexesCreated)
			{
				instance.WorkflowInstances.Indexes.CreateOne(new CreateIndexModel<WorkflowInstance>(
					Builders<WorkflowInstance>.IndexKeys.Ascending(x => x.NextExecution),
					new CreateIndexOptions { Background = true, Name = "idx_nextExec" }));

				instance.Events.Indexes.CreateOne(new CreateIndexModel<Event>(
					Builders<Event>.IndexKeys
						.Ascending(x => x.EventName)
						.Ascending(x => x.EventKey)
						.Ascending(x => x.EventTime),
					new CreateIndexOptions { Background = true, Name = "idx_namekey" }));

				instance.Events.Indexes.CreateOne(new CreateIndexModel<Event>(
					Builders<Event>.IndexKeys.Ascending(x => x.IsProcessed),
					new CreateIndexOptions { Background = true, Name = "idx_processed" }));

				instance.EventSubscriptions.Indexes.CreateOne(new CreateIndexModel<EventSubscription>(
					Builders<EventSubscription>.IndexKeys
						.Ascending(x => x.EventName)
						.Ascending(x => x.EventKey),
					new CreateIndexOptions { Background = true, Name = "idx_namekey" }));

				indexesCreated = true;
			}
		}

		private IMongoCollection<WorkflowInstance> WorkflowInstances => _database.GetCollection<WorkflowInstance>("Workflows");

		private IMongoCollection<EventSubscription> EventSubscriptions => _database.GetCollection<EventSubscription>("Subscriptions");

		private IMongoCollection<Event> Events => _database.GetCollection<Event>("Events");

		private IMongoCollection<ExecutionError> ExecutionErrors => _database.GetCollection<ExecutionError>("ExecutionErrors");

		public bool SupportsScheduledCommands => throw new NotImplementedException();

		public async Task<string> CreateNewWorkflow(WorkflowInstance workflow, CancellationToken cancellationToken = default)
		{
			await WorkflowInstances.InsertOneAsync(workflow, cancellationToken: cancellationToken);
			return workflow.Id;
		}

		public async Task PersistWorkflow(WorkflowInstance workflow, CancellationToken cancellationToken = default)
		{
			await WorkflowInstances.ReplaceOneAsync(x => x.Id == workflow.Id, workflow, cancellationToken: cancellationToken);
		}

		public async Task<IEnumerable<string>> GetRunnableInstances(DateTime asAt, CancellationToken cancellationToken = default)
		{
			long now = asAt.ToUniversalTime().Ticks;
			IFindFluent<WorkflowInstance, string> query = WorkflowInstances
				.Find(x => x.NextExecution.HasValue && (x.NextExecution <= now) && (x.Status == WorkflowStatus.Runnable))
				.Project(x => x.Id);

			return await query.ToListAsync(cancellationToken: cancellationToken);
		}

		public async Task<WorkflowInstance> GetWorkflowInstance(string Id, CancellationToken cancellationToken = default)
		{
			IAsyncCursor<WorkflowInstance> query = await WorkflowInstances.FindAsync(x => x.Id == Id, cancellationToken: cancellationToken);
			WorkflowInstance result = await query.FirstOrDefaultAsync(cancellationToken: cancellationToken);
			if (result == null)
			{
				throw new NotFoundException();
			}

			return result;
		}

		public async Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(IEnumerable<string> ids, CancellationToken cancellationToken = default)
		{
			if (ids == null)
			{
				return new List<WorkflowInstance>();
			}

			IAsyncCursor<WorkflowInstance> result = await WorkflowInstances.FindAsync(x => ids.Contains(x.Id), cancellationToken: cancellationToken);
			return await result.ToListAsync(cancellationToken: cancellationToken);
		}

		public async Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(WorkflowStatus? status, string type, DateTime? createdFrom, DateTime? createdTo, int skip, int take)
		{
			IQueryable<WorkflowInstance> result = WorkflowInstances.AsQueryable();

			if (status.HasValue)
			{
				result = result.Where(x => x.Status == status.Value);
			}

			if (!string.IsNullOrEmpty(type))
			{
				result = result.Where(x => x.WorkflowDefinitionId == type);
			}

			if (createdFrom.HasValue)
			{
				result = result.Where(x => x.CreateTime >= createdFrom.Value);
			}

			if (createdTo.HasValue)
			{
				result = result.Where(x => x.CreateTime <= createdTo.Value);
			}

			return result.Skip(skip).Take(take).ToList();
		}

		public async Task<string> CreateEventSubscription(EventSubscription subscription, CancellationToken cancellationToken = default)
		{
			await EventSubscriptions.InsertOneAsync(subscription, cancellationToken: cancellationToken);
			return subscription.Id;
		}

		public async Task TerminateSubscription(string eventSubscriptionId, CancellationToken cancellationToken = default)
		{
			await EventSubscriptions.DeleteOneAsync(x => x.Id == eventSubscriptionId, cancellationToken: cancellationToken);
		}

		public void EnsureStoreExists()
		{

		}

		public async Task<IEnumerable<EventSubscription>> GetSubscriptions(string eventName, string eventKey, DateTime asOf, CancellationToken cancellationToken = default)
		{
			IFindFluent<EventSubscription, EventSubscription> query = EventSubscriptions
				.Find(x => x.EventName == eventName && x.EventKey == eventKey && x.SubscribeAsOf <= asOf);

			return await query.ToListAsync(cancellationToken: cancellationToken);
		}

		public async Task<string> CreateEvent(Event newEvent, CancellationToken cancellationToken = default)
		{
			await Events.InsertOneAsync(newEvent, cancellationToken: cancellationToken);
			return newEvent.Id;
		}

		public async Task<Event> GetEvent(string id, CancellationToken cancellationToken = default)
		{
			IAsyncCursor<Event> result = await Events.FindAsync(x => x.Id == id, cancellationToken: cancellationToken);
			return await result.FirstAsync(cancellationToken: cancellationToken);
		}

		public async Task<IEnumerable<string>> GetRunnableEvents(DateTime asAt, CancellationToken cancellationToken = default)
		{
			DateTime now = asAt.ToUniversalTime();
			IFindFluent<Event, string> query = Events
				.Find(x => !x.IsProcessed && x.EventTime <= now)
				.Project(x => x.Id);

			return await query.ToListAsync(cancellationToken: cancellationToken);
		}

		public async Task MarkEventProcessed(string id, CancellationToken cancellationToken = default)
		{
			UpdateDefinition<Event> update = Builders<Event>.Update
				.Set(x => x.IsProcessed, true);

			await Events.UpdateOneAsync(x => x.Id == id, update, cancellationToken: cancellationToken);
		}

		public async Task<IEnumerable<string>> GetEvents(string eventName, string eventKey, DateTime asOf, CancellationToken cancellationToken = default)
		{
			IFindFluent<Event, string> query = Events
				.Find(x => x.EventName == eventName && x.EventKey == eventKey && x.EventTime >= asOf)
				.Project(x => x.Id);

			return await query.ToListAsync(cancellationToken: cancellationToken);
		}

		public async Task MarkEventUnprocessed(string id, CancellationToken cancellationToken = default)
		{
			UpdateDefinition<Event> update = Builders<Event>.Update
				.Set(x => x.IsProcessed, false);

			await Events.UpdateOneAsync(x => x.Id == id, update, cancellationToken: cancellationToken);
		}

		public async Task PersistErrors(IEnumerable<ExecutionError> errors, CancellationToken cancellationToken = default)
		{
			if (errors.Any())
			{
				await ExecutionErrors.InsertManyAsync(errors, cancellationToken: cancellationToken);
			}
		}

		public async Task<EventSubscription> GetSubscription(string eventSubscriptionId, CancellationToken cancellationToken = default)
		{
			IAsyncCursor<EventSubscription> result = await EventSubscriptions.FindAsync(x => x.Id == eventSubscriptionId, cancellationToken: cancellationToken);
			return await result.FirstOrDefaultAsync(cancellationToken: cancellationToken);
		}

		public async Task<EventSubscription> GetFirstOpenSubscription(string eventName, string eventKey, DateTime asOf, CancellationToken cancellationToken = default)
		{
			IFindFluent<EventSubscription, EventSubscription> query = EventSubscriptions
				.Find(x => x.EventName == eventName && x.EventKey == eventKey && x.SubscribeAsOf <= asOf && x.ExternalToken == null);

			return await query.FirstOrDefaultAsync(cancellationToken: cancellationToken);
		}

		public async Task<bool> SetSubscriptionToken(string eventSubscriptionId, string token, string workerId, DateTime expiry, CancellationToken cancellationToken = default)
		{
			UpdateDefinition<EventSubscription> update = Builders<EventSubscription>.Update
				.Set(x => x.ExternalToken, token)
				.Set(x => x.ExternalTokenExpiry, expiry)
				.Set(x => x.ExternalWorkerId, workerId);

			UpdateResult result = await EventSubscriptions.UpdateOneAsync(x => x.Id == eventSubscriptionId && x.ExternalToken == null, update, cancellationToken: cancellationToken);
			return (result.ModifiedCount > 0);
		}

		public async Task ClearSubscriptionToken(string eventSubscriptionId, string token, CancellationToken cancellationToken = default)
		{
			UpdateDefinition<EventSubscription> update = Builders<EventSubscription>.Update
				.Set(x => x.ExternalToken, null)
				.Set(x => x.ExternalTokenExpiry, null)
				.Set(x => x.ExternalWorkerId, null);

			await EventSubscriptions.UpdateOneAsync(x => x.Id == eventSubscriptionId && x.ExternalToken == token, update, cancellationToken: cancellationToken);
		}

		public Task ScheduleCommand(ScheduledCommand command)
		{
			throw new NotImplementedException();
		}

		public Task ProcessCommands(DateTimeOffset asOf, Func<ScheduledCommand, Task> action, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}
	}
}
