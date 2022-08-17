using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using ConcernsCasework.Service.MeansOfReferral;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.MeansOfReferral
{
	public sealed class MeansOfReferralCachedService : CachedService, IMeansOfReferralCachedService
	{
		private readonly ILogger<MeansOfReferralCachedService> _logger;
		private readonly IMeansOfReferralService _meansOfReferralService;
		
		private const string MeansOfReferralKey = "Concerns.MeansOfReferral";

		public MeansOfReferralCachedService(ICacheProvider cacheProvider, IMeansOfReferralService meansOfReferralService, ILogger<MeansOfReferralCachedService> logger) 
			: base(cacheProvider)
		{
			_meansOfReferralService = meansOfReferralService;
			_logger = logger;
		}

		public async Task<IList<MeansOfReferralDto>> GetMeansOfReferralsAsync()
		{
			_logger.LogInformation("{ClassName}::{MethodName}", nameof(MeansOfReferralCachedService), nameof(GetMeansOfReferralsAsync));
			
			var referrals = await GetData<IList<MeansOfReferralDto>>(MeansOfReferralKey);
			if (referrals != null) return referrals;

			referrals = await _meansOfReferralService.GetMeansOfReferrals();

			await StoreData(MeansOfReferralKey, referrals);
			
			return referrals;
		}
	}
}