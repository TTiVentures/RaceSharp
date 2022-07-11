using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using WorkflowCore.Models;

namespace RaceSharp.Application.WorkFlowCore.Persistence
{
	public static class ServiceCollectionExtensions
	{
		public static void WorkFlowDefinitionsUseMongoDB(this IServiceCollection services, string mongoUrl, string databaseName)
		{
			MongoClient client = new MongoClient(mongoUrl);
			IMongoDatabase db = client.GetDatabase(databaseName);
			services.AddTransient<IDefinitionRepository, DefinitionRepository>(x => new DefinitionRepository(db));
			services.AddTransient<IResourceRepository, ResourceRepository>(x => new ResourceRepository(db));
		}

		public static WorkflowOptions UseMongoDB(this WorkflowOptions options, string mongoUrl, string databaseName)
		{
			options.UsePersistence(sp =>
			{
				MongoClient client = new MongoClient(mongoUrl);
				IMongoDatabase db = client.GetDatabase(databaseName);
				return new WorkflowPersistenceProvider(db);
			});
			return options;
		}
	}
}
