using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.TRAMS.NtiUnderConsideration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Redis.NtiUnderConsideration
{
	public class NtiUnderConsiderationReasonsCachedService : CachedService, INtiUnderConsiderationReasonsCachedService
	{
		private readonly INtiUnderConsiderationReasonsService _tramsNtiReasonsService;
		private readonly ILogger<NtiUnderConsiderationReasonsCachedService> _logger;

		private const string NtiReasonsCacheKey = "Nti.Reasons";

		public NtiUnderConsiderationReasonsCachedService(ICacheProvider cacheProvider,
			INtiUnderConsiderationReasonsService tramsNtiReasonsService,
			ILogger<NtiUnderConsiderationReasonsCachedService> logger) : base(cacheProvider)
		{
			_tramsNtiReasonsService = tramsNtiReasonsService;
			_logger = logger;
		}

		public async Task ClearData()
		{
			await ClearData(NtiReasonsCacheKey);
		}

		public async Task<ICollection<NtiUnderConsiderationReasonDto>> GetAllReasons()
		{
			_logger.LogInformation("NtiUnderConsiderationReasonsCachedService::GetAllReasons");

			var reasons = await GetData<ICollection<NtiUnderConsiderationReasonDto>>(NtiReasonsCacheKey);
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
