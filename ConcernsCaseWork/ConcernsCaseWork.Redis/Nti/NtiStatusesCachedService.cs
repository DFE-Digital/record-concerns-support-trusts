using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Service.Nti;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Nti
{
	public class NtiStatusesCachedService : CachedService, INtiStatusesCachedService
	{
		private readonly INtiStatusesService _ntiStatusesService;
		private readonly ILogger<NtiStatusesCachedService> _logger;

		private const string CacheKey = "Nti.NoticeToImprove.Statuses";

		public NtiStatusesCachedService(ICacheProvider cacheProvider,
			INtiStatusesService ntiStatusesService,
			ILogger<NtiStatusesCachedService> logger) 
			: base(cacheProvider)
		{
			_ntiStatusesService = ntiStatusesService;
			_logger = logger;
		}

		public async Task ClearData()
		{
			await ClearData(CacheKey);
		}

		public async Task<ICollection<NtiStatusDto>> GetAllStatusesAsync()
		{
			_logger.LogInformation("NtiStatusesCachedService::GetAllStatusesAsync");

			var statuses = await GetData<ICollection<NtiStatusDto>>(CacheKey);
			if (statuses == null || statuses.Count == 0)
			{
				statuses = await _ntiStatusesService.GetNtiStatusesAsync();
				if (statuses?.Count > 0)
				{
					await StoreData(CacheKey, statuses);
				}
			}

			return statuses;
		}
	}
}
