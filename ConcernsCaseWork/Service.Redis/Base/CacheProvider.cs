using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.Redis.Base
{
	public sealed class CacheProvider : ICacheProvider
	{
		private readonly IDistributedCache _cache;
		private const int CacheTtl = 120;

		public CacheProvider(IDistributedCache cache)
		{
			_cache = cache;
		}

		public int CacheTimeToLive()
		{
			return CacheTtl;
		}

		public async Task<T> GetFromCache<T>(string key) where T : class
		{
			var cachedUsers = await _cache.GetStringAsync(key);
			return cachedUsers == null ? null : JsonSerializer.Deserialize<T>(cachedUsers);
		}

		public async Task SetCache<T>(string key, T value, DistributedCacheEntryOptions options) where T : class
		{
			var user = JsonSerializer.Serialize(value);
			await _cache.SetStringAsync(key, user , options);
		}

		public async Task ClearCache(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}
			await _cache.RemoveAsync(key);
		}
	}
}