using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.TRAMS.FinancialPlan;
using Service.TRAMS.Ratings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Redis.FinancialPlan
{
	public sealed class FinancialPlanStatusCachedService : CachedService, IFinancialPlanStatusCachedService
	{
		private readonly ILogger<FinancialPlanStatusCachedService> _logger;
		private readonly IFinancialPlanStatusService _financialPlanStatusService;

		private const string FinancialPlanStatusKey = "FinancialPlan.Statuses";
		
		public FinancialPlanStatusCachedService(ICacheProvider cacheProvider, IFinancialPlanStatusService financialPlanStatusService, ILogger<FinancialPlanStatusCachedService> logger) 
			: base(cacheProvider)
		{
			_financialPlanStatusService = financialPlanStatusService;
			_logger = logger;
		}

		public async Task ClearData()
		{
			await ClearData(FinancialPlanStatusKey);
		}

		public async Task<IList<FinancialPlanStatusDto>> GetFinancialPlanStatuses()
		{
			_logger.LogInformation("FinancialPlanStatusCachedService::GetFinancialPlanStatuses");
			
			// Check cache
			var financialPlanStatuses = await GetData<IList<FinancialPlanStatusDto>>(FinancialPlanStatusKey);
			if (financialPlanStatuses != null) return financialPlanStatuses;

			// Fetch from Academies API
			financialPlanStatuses = await _financialPlanStatusService.GetFinancialPlansStatuses();

			// Store in cache for 24 hours (default)
			await StoreData(FinancialPlanStatusKey, financialPlanStatuses);
			
			return financialPlanStatuses;
		}

		public async Task<FinancialPlanStatusDto> GetDefaultFinancialPlan()
		{
			var financialPlanStatusesDto = await GetFinancialPlanStatuses();
			var financialPlanStatusDto = financialPlanStatusesDto.FirstOrDefault(r => r.Name.Equals("Unknown"));

			return financialPlanStatusDto;
		}
	}
}