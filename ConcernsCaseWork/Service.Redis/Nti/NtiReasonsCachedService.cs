using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.TRAMS.Nti;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Redis.Nti
{
	public class NtiReasonsCachedService : CachedService, INtiReasonsCachedService
	{
		private readonly INtiReasonsService _tramsNtiReasonsService;
		private readonly ILogger<NtiReasonsCachedService> _logger;

		private const string NtiReasonsCacheKey = "Nti.Reasons";

		public NtiReasonsCachedService(INtiReasonsService tramsNtiReasonsService,
			ICacheProvider cacheProvider,
			ILogger<NtiReasonsCachedService> logger) : base(cacheProvider)
		{
			_tramsNtiReasonsService = tramsNtiReasonsService;
			_logger = logger;
		}

		public async Task<ICollection<NtiReasonDto>> GetAllReasons()
		{
			var reasons = await GetData<ICollection<NtiReasonDto>>(NtiReasonsCacheKey);
			if(reasons == null || reasons.Count == 0)
			{
				reasons = await _tramsNtiReasonsService.GetAllReasons();
				if (reasons?.Count > 0)
				{
					await StoreData(NtiReasonsCacheKey, reasons);
				}
			}

			return reasons;
		}
	}
}
