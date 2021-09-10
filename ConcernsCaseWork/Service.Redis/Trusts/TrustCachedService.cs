using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.TRAMS.Trusts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Trusts
{
	public sealed class TrustCachedService : CachedService, ITrustCachedService
	{
		private readonly ILogger<TrustCachedService> _logger;
		private readonly ITrustService _trustService;

		private const string TrustsKey = "Trusts";
		
		public TrustCachedService(ICacheProvider cacheProvider, ITrustService trustService, ILogger<TrustCachedService> logger) 
			: base(cacheProvider)
		{
			_trustService = trustService;
			_logger = logger;
		}
		
		public async Task<TrustDetailsDto> GetTrustByUkPrn(string ukPrn)
		{
			_logger.LogInformation("TrustCachedService::GetTrustByUkPrn");
			
			// Check cache
			var trustsCached = await GetData<IDictionary<string, TrustDetailsDto>>(TrustsKey);
			if (trustsCached != null && trustsCached.ContainsKey(ukPrn) && trustsCached.TryGetValue(ukPrn, out var trustDetailsDto))
			{
				return trustDetailsDto;
			}
			
			// Fetch from TRAMS API
			trustDetailsDto = await _trustService.GetTrustByUkPrn(ukPrn);

			// Store in cache for 24 hours (default)
			if (trustsCached is null)
			{
				trustsCached = new Dictionary<string, TrustDetailsDto> { { ukPrn, trustDetailsDto } };
			}
			else
			{
				trustsCached.Add(ukPrn, trustDetailsDto);
			}
			await StoreData(TrustsKey, trustsCached);
			
			return trustDetailsDto;
		}
	}
}