using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Service.Redis.Configuration;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.Redis.Base
{
	public sealed class CacheProvider : ICacheProvider
	{
		private readonly IDistributedCache _cache;
		private readonly int _cacheTtl;

		public CacheProvider(IDistributedCache cache, IOptions<CacheOptions> options)
		{
			_cache = cache;
			_cacheTtl = options.Value.TimeToLive;
		}

		public int CacheTimeToLive()
		{
			return _cacheTtl;
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