using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;

namespace RaceSharp.Application
{
	public class CacheService : ICacheService
	{
		private readonly ConfigurationOptions? configuration = null;
		private Lazy<IConnectionMultiplexer>? _Connection = null;
		private int _Database = 0;

		public CacheService(CacheServiceSettings settings)
		{
			_Database = settings.Database;
			configuration = new ConfigurationOptions()
			{
				Ssl = settings.Ssl,
				EndPoints = { { settings.Host, settings.Port }, },
				AllowAdmin = settings.AllowAdmin,
				Password = settings.Password,
				ClientName = settings.ClientName,
				ReconnectRetryPolicy = new LinearRetry(5000),
				AbortOnConnectFail = false,
			};
			_Connection = new Lazy<IConnectionMultiplexer>(() =>
			{
				ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(configuration);
				redis.ErrorMessage += _Connection_ErrorMessage;
				//redis.InternalError += _Connection_InternalError;
				//redis.ConnectionFailed += _Connection_ConnectionFailed;
				//redis.ConnectionRestored += _Connection_ConnectionRestored;
				return redis;
			});
		}

		// For the 'GetSubscriber()' and another Databases
		public IConnectionMultiplexer Connection { get { return _Connection!.Value; } }

		// For the default database
		public IDatabase Database => Connection.GetDatabase(_Database);

		private void _Connection_ErrorMessage(object sender, RedisErrorEventArgs e)
		{
			Log.Error($"Redis Error connection {e.EndPoint}.\n{e.Message}");
		}

		public async Task<T?> Get<T>(string key, CommandFlags flags = CommandFlags.None)
		{
			var redisValue = await Database.StringGetAsync(key, flags);
			if (!redisValue.HasValue)
			{
				return default;
			}

			try
			{
				var @object = JsonConvert.DeserializeObject<T>(redisValue);
				return @object;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}

			return default;
		}

		public async Task<bool> Set<T>(string key, T value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
		{
			try
			{
				var valueSerialized = JsonConvert.SerializeObject(value);
				return await Database.StringSetAsync(key, valueSerialized, expiry, when, flags);
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}

			return false;
		}

		public async Task<bool> RemoveKey(string key, CommandFlags flags = CommandFlags.None)
		{
			try
			{
				return await Database.KeyDeleteAsync(key, flags);
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}

			return false;
		}


		public async Task<IEnumerable<T?>?> GetSortedSetByRank<T>(string key, int start = 0, int end = -1, Order order = Order.Ascending)
		{
			var array = await Database.SortedSetRangeByRankAsync(key: key, start: start, stop: end, order: order);

			try
			{
				var list = array.Select(x => JsonConvert.DeserializeObject<T>(x));
				return list;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}

			return null;
		}

		public async Task<IEnumerable<T?>?> GetSortedSetByValue<T>(string key, Order order = Order.Ascending)
		{
			var array = await Database.SortedSetRangeByValueAsync(key: key, order: order);

			try
			{
				var list = array.Select(x => JsonConvert.DeserializeObject<T>(x));
				return list;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}

			return null;
		}

		public async Task<bool> AddToSortedSet<T>(string key, T value, double score = 0, When when = When.Always, CommandFlags flags = CommandFlags.None)
		{
			await RemoveKeyIfNotMachingType(key, RedisType.SortedSet);
			try
			{
				var valueSerialized = JsonConvert.SerializeObject(value);
				return await Database.SortedSetAddAsync(key, valueSerialized, score, when, flags);
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}

			return false;
		}

		public async Task<bool> RemoveFromSortedSet<T>(string key, T value, CommandFlags flags = CommandFlags.None)
		{
			try
			{
				var valueSerialized = JsonConvert.SerializeObject(value);
				return await Database.SortedSetRemoveAsync(key, valueSerialized, flags);
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}

			return false;
		}

		public async Task<bool> AddToSet<T>(string key, T value, CommandFlags flags = CommandFlags.None)
		{
			await RemoveKeyIfNotMachingType(key, RedisType.Set);
			try
			{
				var valueSerialized = JsonConvert.SerializeObject(value);
				return await Database.SetAddAsync(key, valueSerialized, flags);
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}

			return false;
		}

		public async Task<bool> RemoveFromdSet<T>(string key, T value, CommandFlags flags = CommandFlags.None)
		{
			try
			{
				var valueSerialized = JsonConvert.SerializeObject(value);
				return await Database.SetRemoveAsync(key, valueSerialized, flags);
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}

			return false;
		}


		private async Task RemoveKeyIfNotMachingType(string key, RedisType type)
		{
			if (Database.KeyType(key) != type)
			{
				await RemoveKey(key);
			}
		}

	}

}
