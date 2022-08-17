using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using ConcernsCasework.Service.NtiWarningLetter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.NtiWarningLetter
{
	public class NtiWarningLetterStatusesCachedService : CachedService, INtiWarningLetterStatusesCachedService
	{
		private readonly INtiWarningLetterStatusesService _ntiWarningLetterStatusesService;
		private readonly ILogger<NtiWarningLetterStatusesCachedService> _logger;

		private const string CacheKey = "Nti.WarningLetter.Statuses";

		public NtiWarningLetterStatusesCachedService(ICacheProvider cacheProvider, INtiWarningLetterStatusesService ntiWarningLetterStatusesService,
			ILogger<NtiWarningLetterStatusesCachedService> logger) : base(cacheProvider)
		{
			_ntiWarningLetterStatusesService = ntiWarningLetterStatusesService;
			_logger = logger;
		}

		public async Task ClearData()
		{
			await ClearData(CacheKey);
		}

		public async Task<ICollection<NtiWarningLetterStatusDto>> GetAllStatusesAsync()
		{
			_logger.LogInformation("NtiWarningLetterStatusesCachedService::GetAllStatusesAsync");

			var statuses = await GetData<ICollection<NtiWarningLetterStatusDto>>(CacheKey);
			if(statuses == null || statuses.Count == 0)
			{
				statuses = await _ntiWarningLetterStatusesService.GetAllStatusesAsync();
				if(statuses?.Count > 0)
				{
					await StoreData(CacheKey, statuses);	
				}
			}

			return statuses;
		}
	}
}
