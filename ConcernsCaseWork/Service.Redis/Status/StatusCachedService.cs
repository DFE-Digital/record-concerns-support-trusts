using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using ConcernsCasework.Service.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Redis.Status
{
	public sealed class StatusCachedService : CachedService, IStatusCachedService
	{
		private readonly ILogger<StatusCachedService> _logger;
		private readonly IStatusService _statusService;

		private const string StatusesKey = "Concerns.Statuses";
		
		public StatusCachedService(ICacheProvider cacheProvider, IStatusService statusService, ILogger<StatusCachedService> logger) 
			: base(cacheProvider)
		{
			_statusService = statusService;
			_logger = logger;
		}

		public async Task ClearData()
		{
			await ClearData(StatusesKey);
		}

		public async Task<IList<StatusDto>> GetStatuses()
		{
			_logger.LogInformation("StatusCachedService::GetStatuses");
			
			// Check cache
			var statuses = await GetData<IList<StatusDto>>(StatusesKey);
			if (statuses != null) return statuses;

			// Fetch from Academies API
			statuses = await _statusService.GetStatuses();

			// Store in cache for 24 hours (default)
			await StoreData(StatusesKey, statuses);
			
			return statuses;
		}

		public async Task<StatusDto> GetStatusByName(string name)
		{
			_logger.LogInformation("StatusCachedService::GetStatusByName {Name}", name);

			var statuses = await GetStatuses();

			return statuses.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}
	}
}