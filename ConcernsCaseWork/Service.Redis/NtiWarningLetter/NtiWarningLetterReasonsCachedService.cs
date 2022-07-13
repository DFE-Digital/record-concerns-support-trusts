using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.TRAMS.NtiWarningLetter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Redis.NtiWarningLetter
{
	public class NtiWarningLetterReasonsCachedService : CachedService, INtiWarningLetterReasonsCachedService
	{
		private readonly INtiWarningLetterReasonsService _ntiWarningLetterReasonsService;
		private readonly ILogger<NtiWarningLetterReasonsCachedService> _logger;

		private const string CacheKey = "Nti.WarningLetter.Reasons";

		public NtiWarningLetterReasonsCachedService(ICacheProvider cacheProvider, INtiWarningLetterReasonsService ntiWarningLetterReasonsService,
			ILogger<NtiWarningLetterReasonsCachedService> logger) : base(cacheProvider)
		{
			_ntiWarningLetterReasonsService = ntiWarningLetterReasonsService;
			_logger = logger;
		}

		public async Task<ICollection<NtiWarningLetterReasonDto>> GetAllReasonsAsync()
		{
			var reasons = await GetData<ICollection<NtiWarningLetterReasonDto>>(CacheKey);
			if (reasons == null || reasons.Count == 0)
			{
				reasons = await _ntiWarningLetterReasonsService.GetAllReasonsAsync();
				if (reasons?.Count > 0)
				{
					await StoreData(CacheKey, reasons);
				}
			}

			return reasons;
		}
	}
}
