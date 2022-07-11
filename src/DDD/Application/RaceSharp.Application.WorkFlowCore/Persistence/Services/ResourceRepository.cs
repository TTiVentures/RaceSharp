using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Linq;

namespace RaceSharp.Application.WorkFlowCore.Persistence
{
	public class ResourceRepository : IResourceRepository
	{
		private readonly IMongoDatabase _database;

		private IMongoCollection<StoredResource> _collection => _database.GetCollection<StoredResource>("Resources");

		static ResourceRepository()
		{
		}

		public ResourceRepository(IMongoDatabase database)
		{
			_database = database;
			CreateIndexes(_collection);
		}

		public Resource Find(Bucket bucket, string name)
		{
			int? version = GetLatestVersion(bucket, name);
			if (version == null)
			{
				return null;
			}

			return Find(bucket, name, version.Value);
		}

		public Resource Find(Bucket bucket, string name, int version)
		{
			IFindFluent<StoredResource, StoredResource> result = _collection.Find(x => x.Bucket == bucket && x.Name == name && x.Version == version);
			if (!result.Any())
			{
				return null;
			}

			string json = result.First().Resource.ToJson();
			return JsonConvert.DeserializeObject<Resource>(json);
		}

		public int? GetLatestVersion(Bucket bucket, string name)
		{
			IQueryable<StoredResource> versions = _collection.AsQueryable().Where(x => x.Bucket == bucket && x.Name == name);
			if (!versions.Any())
			{
				return null;
			}

			return versions.Max(x => x.Version);
		}

		public void Save(Bucket bucket, Resource resource)
		{
			string json = JsonConvert.SerializeObject(resource);
			BsonDocument doc = BsonDocument.Parse(json);

			int version = GetLatestVersion(bucket, resource.Name) ?? 1;

			_collection.InsertOne(new StoredResource()
			{
				Name = resource.Name,
				Version = version,
				Bucket = bucket,
				Resource = doc
			});
		}

		private static bool indexesCreated = false;

		private static void CreateIndexes(IMongoCollection<StoredResource> collection)
		{
			if (!indexesCreated)
			{
				var keys1 = Builders<StoredResource>.IndexKeys
					.Ascending(x => x.Bucket)
					.Ascending(x => x.Name)
					.Ascending(x => x.Version);
				var indexOptions1 = new CreateIndexOptions { Background = true, Name = "unq_resource_id_version", Unique = true };
				collection.Indexes.CreateOne(new CreateIndexModel<StoredResource>(keys1, indexOptions1));

				var keys2 = Builders<StoredResource>.IndexKeys
					.Ascending(x => x.Name);
				var indexOptions2 = new CreateIndexOptions { Background = true, Name = "idx_resource_id" };
				collection.Indexes.CreateOne(new CreateIndexModel<StoredResource>(keys2, indexOptions2));

				indexesCreated = true;
			}
		}
	}
}
