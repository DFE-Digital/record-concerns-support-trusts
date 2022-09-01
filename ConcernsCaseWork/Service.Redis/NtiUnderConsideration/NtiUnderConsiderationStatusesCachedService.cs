using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.TRAMS.NtiUnderConsideration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.NtiUnderConsideration
{
	public class NtiUnderConsiderationStatusesCachedService : CachedService, INtiUnderConsiderationStatusesCachedService
	{
		private readonly INtiUnderConsiderationStatusesService _tramsNtiStatusesService;
		private readonly ILogger<NtiUnderConsiderationStatusesCachedService> _logger;

		private const string NtiStatusesCacheKey = "Nti.UnderConsideration.Statuses";

		public NtiUnderConsiderationStatusesCachedService(ICacheProvider cacheProvider,
			INtiUnderConsiderationStatusesService tramsNtiStatusesService,
			ILogger<NtiUnderConsiderationStatusesCachedService> logger) : base(cacheProvider)
		{
			_tramsNtiStatusesService = tramsNtiStatusesService;
			_logger = logger;
		}

		public async Task ClearData()
		{
			await ClearData(NtiStatusesCacheKey);
		}

		public async Task<ICollection<NtiUnderConsiderationStatusDto>> GetAllStatuses()
		{
			_logger.LogInformation("NtiUnderConsiderationStatusesCachedService::GetAllStatuses");

			var statuses = await GetData<ICollection<NtiUnderConsiderationStatusDto>>(NtiStatusesCacheKey);
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
