using System.Text.Json;

namespace RaceSharp
{
	public static class JsonExtensions
	{
		public static string ToJson(this object obj, JsonSerializerOptions? options = null)
		{
			return JsonSerializer.Serialize(obj, options);
		}

		public static string ToJsonIndented(this object obj) {
			return obj.ToJson(new() { WriteIndented = true, });
		}

		public static T? JsonToObject<T>(this string json, JsonSerializerOptions? options = null)
		{
			return JsonSerializer.Deserialize<T>(json, options);
		}
	}
}