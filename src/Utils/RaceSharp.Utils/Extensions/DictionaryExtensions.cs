namespace Eetee.Client;

public static class DictionaryExtensions
{
	public static T? TryGetValueOrDefault<T>(this Dictionary<string, T> dictionary, string key)
	{
		return dictionary.ContainsKey(key) ? dictionary[key] : default;
	}

	public static string TryGetValueAsString<T>(this Dictionary<string, T> dictionary, string key)
	{
		return dictionary.TryGetValue(key, out var value) ? value.ToStringSafe() : string.Empty;
	}
}

public static class StringExtensions
{
	public static string ToStringSafe(this object? value)
	{
		var returnValue = value?.ToString();
		return returnValue ?? string.Empty;
	}
}