using System.Text.Json;

namespace RaceSharp
{
	public static class JsonExtensions
	{
		public static string ToJson(this object @object, JsonSerializerOptions? options = null)
		{
			return JsonSerializer.Serialize(@object, options);
		}

		public static T? JsonToObject<T>(this string json, JsonSerializerOptions? options = null)
		{
			return JsonSerializer.Deserialize<T>(json, options);
		}

		public static async Task<string> ToJsonAsync(this object obj)
		{
			// https://stackoverflow.com/questions/58469794/c-net-core3-0-system-text-json-jsonserializer-serializeasync

			string json = string.Empty;
			using (var stream = new MemoryStream())
			{
				await JsonSerializer.SerializeAsync(stream, obj, obj.GetType());
				stream.Position = 0;
				using var reader = new StreamReader(stream);
				return await reader.ReadToEndAsync();
			}
		}
	}
}