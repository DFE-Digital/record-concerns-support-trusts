﻿using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Service.NtiWarningLetter;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.NtiWarningLetter
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

		public async Task ClearData()
		{
			await ClearData(CacheKey);
		}

		public async Task<ICollection<NtiWarningLetterReasonDto>> GetAllReasonsAsync()
		{
			_logger.LogInformation("NtiWarningLetterReasonsCachedService::GetAllReasonsAsync");

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