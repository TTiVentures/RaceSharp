using MongoDB.Bson;

namespace RaceSharp.Application.WorkFlowCore.Persistence
{
	public class StoredResource
	{
		public ObjectId Id { get; set; }

		public Bucket Bucket { get; set; }

		public string Name { get; set; }

		public int Version { get; set; }

		public BsonDocument Resource { get; set; }
	}
}
