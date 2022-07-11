using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RaceSharp.Application.WorkFlowCore.Persistence
{
	public class DataObjectSerializer : SerializerBase<object>
	{
		private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
		{
			TypeNameHandling = TypeNameHandling.None,
		};

		public override object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			string result = BsonSerializer.Deserialize<string>(context.Reader);

			JObject obj = JObject.Parse(result);
			//return (obj as IDictionary<string, object>);
			return obj;

			//return JObject.Parse(result).t
			//return JObject.FromObject(result);
		}

		public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
		{
			string str = JsonConvert.SerializeObject(value, SerializerSettings);
			//var doc = BsonDocument.Parse(str);
			MongoDB.Bson.Serialization.

			BsonSerializer.Serialize(context.Writer, str);
		}

	}
}
