using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.TRAMS.Models;
using Service.TRAMS.Trusts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Redis.Trusts
{
	public sealed class TrustCachedService : ITrustCachedService
	{
		private readonly ITrustService _trustService;
		private readonly ICacheProvider _cacheProvider;
		private readonly ILogger<TrustCachedService> _logger;
		
		private const string CacheKey = "trusts";
		
		public TrustCachedService(ITrustService trustService, ICacheProvider cacheProvider, ILogger<TrustCachedService> logger)
		{
			_trustService = trustService;
			_cacheProvider = cacheProvider;
			_logger = logger;
		}

		public async Task<IList<TrustDto>> GetTrustsCached()
		{
			_logger.LogInformation("TrustCachedService::GetTrustsCached execution");
			
			var trustsCached = await _cacheProvider.GetFromCache<List<TrustDto>>(CacheKey);
			if (trustsCached != null) return trustsCached;

			var trustList = new List<TrustDto>();
			IList<TrustDto> trusts;
			int page = 0;
				
			do
			{
				trusts = await _trustService.GetTrustsByPagination(++page);
				if (trusts.Any()) {
					trustList.AddRange(trusts);
				}
				
			} while (trusts.Any());
			
			if (!trustList.Any()) return trustList;
			
			// Sort after fetching all trusts and stored in cache
			trustList.Sort();
			
			// Only store in cache if trusts exist
			var cacheEntryOptions = new DistributedCacheEntryOptions()
				.SetSlidingExpiration(TimeSpan.FromSeconds(_cacheProvider.CacheTimeToLive())); 
	                
			await _cacheProvider.SetCache(CacheKey, trustList, cacheEntryOptions);

			return trustList;
		}
	}
}