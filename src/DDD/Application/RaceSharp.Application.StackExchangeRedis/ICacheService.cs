using StackExchange.Redis;

namespace RaceSharp.Application
{
	public interface ICacheService
	{
		IConnectionMultiplexer Connection { get; }
		IDatabase Database { get; }

		Task<bool> AddToSet<T>(string key, T value, CommandFlags flags = CommandFlags.None);
		Task<bool> AddToSortedSet<T>(string key, T value, double score = 0, When when = When.Always, CommandFlags flags = CommandFlags.None);
		Task<T?> Get<T>(string key, CommandFlags flags = CommandFlags.None);
		Task<IEnumerable<T?>?> GetSortedSetByRank<T>(string key, int start = 0, int end = -1, Order order = Order.Ascending);
		Task<IEnumerable<T?>?> GetSortedSetByValue<T>(string key, Order order = Order.Ascending);
		Task<bool> RemoveFromdSet<T>(string key, T value, CommandFlags flags = CommandFlags.None);
		Task<bool> RemoveFromSortedSet<T>(string key, T value, CommandFlags flags = CommandFlags.None);
		Task<bool> RemoveKey(string key, CommandFlags flags = CommandFlags.None);
		Task<bool> Set<T>(string key, T value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None);
	}
}