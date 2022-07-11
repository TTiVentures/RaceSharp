using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace RaceSharp.Application.WorkFlowCore.Persistence
{
	public class DefinitionRepository : IDefinitionRepository
	{
		private readonly IMongoDatabase _database;

		private IMongoCollection<StoredDefinition> Collection => _database.GetCollection<StoredDefinition>("Definitions");

		static DefinitionRepository()
		{
			//BsonSerializer.RegisterSerializer(typeof(JObject), new DataObjectSerializer());
			//Dictionary<string, object>
		}

		public DefinitionRepository(IMongoDatabase database)
		{
			_database = database;
			CreateIndexes(Collection);
		}

		public Definition Find(string workflowId)
		{
			int? version = GetLatestVersion(workflowId);
			if (version == null)
			{
				return null;
			}

			return Find(workflowId, version.Value);
		}

		public Definition Find(string workflowId, int version)
		{
			IFindFluent<StoredDefinition, StoredDefinition> result = Collection.Find(x => x.ExternalId == workflowId && x.Version == version);
			if (!result.Any())
			{
				return null;
			}

			string json = result.First().Definition.ToJson();
			return JsonConvert.DeserializeObject<Definition>(json);
		}

		public int? GetLatestVersion(string workflowId)
		{
			IQueryable<StoredDefinition> versions = Collection.AsQueryable().Where(x => x.ExternalId == workflowId);
			if (!versions.Any())
			{
				return null;
			}

			return versions.Max(x => x.Version);
		}

		public IEnumerable<Definition> GetAll()
		{
			IQueryable<BsonDocument> results = Collection.AsQueryable().Select(x => x.Definition);

			foreach (BsonDocument item in results)
			{
				string json = item.ToJson();
				yield return JsonConvert.DeserializeObject<Definition>(json);
			}
		}

		public void Save(Definition definition)
		{
			string json = JsonConvert.SerializeObject(definition);
			BsonDocument doc = BsonDocument.Parse(json);

			if (Collection.AsQueryable().Any(x => x.ExternalId == definition.Id && x.Version == definition.Version))
			{
				Collection.ReplaceOne(x => x.ExternalId == definition.Id && x.Version == definition.Version, new StoredDefinition()
				{
					ExternalId = definition.Id,
					Version = definition.Version,
					Definition = doc
				});
				return;
			}

			Collection.InsertOne(new StoredDefinition()
			{
				ExternalId = definition.Id,
				Version = definition.Version,
				Definition = doc
			});
		}

		private static bool indexesCreated = false;

		private static void CreateIndexes(IMongoCollection<StoredDefinition> collection)
		{
			if (!indexesCreated)
			{
				var keys1 = Builders<StoredDefinition>.IndexKeys
					.Ascending(x => x.ExternalId)
					.Ascending(x => x.Version);
				var indexOptions1 = new CreateIndexOptions { Background = true, Name = "unq_definition_id_version", Unique = true };
				collection.Indexes.CreateOne(new CreateIndexModel<StoredDefinition>(keys1, indexOptions1));

				var keys2 = Builders<StoredDefinition>.IndexKeys
					.Ascending(x => x.ExternalId);
				var indexOptions2 = new CreateIndexOptions { Background = true, Name = "idx_definition_id" };
				collection.Indexes.CreateOne(new CreateIndexModel<StoredDefinition>(keys2, indexOptions2));

				indexesCreated = true;
			}
		}
	}
}
