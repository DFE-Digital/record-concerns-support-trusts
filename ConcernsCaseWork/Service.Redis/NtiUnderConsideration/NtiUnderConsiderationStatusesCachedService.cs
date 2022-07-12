using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.TRAMS.Nti;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Redis.NtiUnderConsideration
{
	public class NtiUnderConsiderationStatusesCachedService : CachedService, INtiUnderConsiderationStatusesCachedService
	{
		private readonly INtiStatusesService _tramsNtiStatusesService;
		private readonly ILogger<NtiUnderConsiderationStatusesCachedService> _logger;

		private const string NtiStatusesCacheKey = "Nti.Statuses";

		public NtiUnderConsiderationStatusesCachedService(ICacheProvider cacheProvider,
			INtiStatusesService tramsNtiStatusesService,
			ILogger<NtiUnderConsiderationStatusesCachedService> logger) : base(cacheProvider)
		{
			_tramsNtiStatusesService = tramsNtiStatusesService;
			_logger = logger;
		}

		public async Task<ICollection<NtiStatusDto>> GetAllStatuses()
		{
			var statuses = await GetData<ICollection<NtiStatusDto>>(NtiStatusesCacheKey);
			if (statuses == null || statuses.Count == 0)
			{
				statuses = await _tramsNtiStatusesService.GetAllStatuses();
				if (statuses?.Count > 0)
				{
					await StoreData(NtiStatusesCacheKey, statuses);
				}
			}

			return statuses;
		}
	}
}
