using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;

namespace Service.Redis.Base
{
	public class CachedService : ICachedService
	{
		private readonly ICacheProvider _cacheProvider;

		public CachedService(ICacheProvider cacheProvider)
		{
			_cacheProvider = cacheProvider;
		}
		
		public async Task StoreData<T>(string key, T data, int expirationTimeInHours = 24) where T : class
		{
			var cacheEntryOptions = new DistributedCacheEntryOptions()
				.SetSlidingExpiration(TimeSpan.FromHours(expirationTimeInHours));
			await _cacheProvider.SetCache(key, data, cacheEntryOptions);
		}
		
		public async Task<T> GetData<T>(string key) where T : class
		{
			return await _cacheProvider.GetFromCache<T>(key);
		}

		public async Task ClearData(string key)
		{
			await _cacheProvider.ClearCache(key);
		}
	}
}