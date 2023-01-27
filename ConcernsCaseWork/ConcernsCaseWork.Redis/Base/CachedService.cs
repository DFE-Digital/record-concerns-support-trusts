using Ardalis.GuardClauses;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Base
{
	public abstract class CachedService : ICachedService
	{
		private readonly ICacheProvider _cacheProvider;

		protected CachedService(ICacheProvider cacheProvider)
		{
			_cacheProvider = cacheProvider;
		}

		// Todo. scope these methods?

		public Task StoreData<T>(string key, T data, int expirationTimeInHours = 24) where T : class
		{
			Guard.Against.NullOrWhiteSpace(key);
			Guard.Against.Null(data);
			Guard.Against.NegativeOrZero(expirationTimeInHours);

			Task DoWork()
			{
				var cacheEntryOptions = new DistributedCacheEntryOptions()
					.SetSlidingExpiration(TimeSpan.FromHours(expirationTimeInHours));
				return _cacheProvider.SetCache(key, data, cacheEntryOptions);
			}
			return DoWork();
		}

		public Task<T> GetData<T>(string key) where T : class
		{
			Guard.Against.NullOrWhiteSpace(key);
			return _cacheProvider.GetFromCache<T>(key);
		}

		public Task ClearData(string key)
		{
			Guard.Against.NullOrWhiteSpace(key);
			return _cacheProvider.ClearCache(key);
		}
	}
}