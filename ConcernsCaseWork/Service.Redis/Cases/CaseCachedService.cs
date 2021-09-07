using Microsoft.Extensions.Caching.Distributed;
using Service.Redis.Base;
using System;
using System.Threading.Tasks;

namespace Service.Redis.Cases
{
	public sealed class CaseCachedService : ICaseCachedService
	{
		private readonly ICacheProvider _cacheProvider;
		private const int CaseStateDateExpiration = 24;
		
		public CaseCachedService(ICacheProvider cacheProvider)
		{
			_cacheProvider = cacheProvider;
		}
		
		public async Task CreateCaseData<T>(string key, T data) where T : class
		{
			var cacheEntryOptions = new DistributedCacheEntryOptions()
				.SetSlidingExpiration(TimeSpan.FromHours(CaseStateDateExpiration));
			await _cacheProvider.SetCache(key, data, cacheEntryOptions);
		}

		public async Task<T> GetCaseData<T>(string key) where T : class
		{
			return await _cacheProvider.GetFromCache<T>(key);
		}
	}
}