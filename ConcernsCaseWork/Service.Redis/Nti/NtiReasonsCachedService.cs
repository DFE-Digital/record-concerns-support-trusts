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
		private const string CacheKey = "Nti.NoticeToImprove.Reasons";
		private readonly INtiReasonsService _ntiReasonsService;
		private readonly ILogger<NtiReasonsCachedService> _logger;

		public NtiReasonsCachedService(ICacheProvider cacheProvider,
			INtiReasonsService ntiReasonsService,
			ILogger<NtiReasonsCachedService> logger) : base(cacheProvider)
		{
			_ntiReasonsService = ntiReasonsService;
			_logger = logger;
		}

		public async Task ClearData()
		{
			await ClearData(CacheKey);
		}

		public async Task<ICollection<NtiReasonDto>> GetAllReasonsAsync()
		{
			_logger.LogInformation("NtiReasonsCachedService::GetAllReasonsAsync");

			var reasons = await GetData<ICollection<NtiReasonDto>>(CacheKey);
			if (reasons == null || reasons.Count == 0)
			{
				reasons = await _ntiReasonsService.GetNtiReasonsAsync();
				if (reasons?.Count > 0)
				{
					await StoreData(CacheKey, reasons);
				}
			}

			return reasons;
		}
	}
}
