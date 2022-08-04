using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.TRAMS.FinancialPlan;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.FinancialPlan
{
	public sealed class FinancialPlanStatusCachedService : CachedService, IFinancialPlanStatusCachedService
	{
		private readonly ILogger<FinancialPlanStatusCachedService> _logger;
		private readonly IFinancialPlanStatusService _financialPlanStatusService;

		private const string FinancialPlanStatusKey = "FinancialPlan.Statuses";
		private const string FinancialPlanClosureStatusKey = "FinancialPlan.ClosureStatuses";
		private const string FinancialPlanOpenStatusKey = "FinancialPlan.OpenStatuses";
		
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

		public async Task<IList<FinancialPlanStatusDto>> GetAllFinancialPlanStatusesAsync()
		{
			_logger.LogInformation("FinancialPlanStatusCachedService::GetAllFinancialPlanStatuses");
			
			return await GetFinancialPlansStatusesAsync(
				async () => await _financialPlanStatusService.GetAllFinancialPlansStatuses(), 
				FinancialPlanStatusKey);
		}

		public async Task<IList<FinancialPlanStatusDto>> GetClosureFinancialPlansStatusesAsync()
		{
			_logger.LogInformation("FinancialPlanStatusCachedService::GetClosureFinancialPlansStatuses");
			
			return await GetFinancialPlansStatusesAsync(
				async () => await _financialPlanStatusService.GetClosureFinancialPlansStatuses(), 
				FinancialPlanClosureStatusKey);
		}

		public async Task<IList<FinancialPlanStatusDto>> GetOpenFinancialPlansStatusesAsync()
		{
			_logger.LogInformation("FinancialPlanStatusCachedService::GetOpenFinancialPlansStatuses");
			
			return await GetFinancialPlansStatusesAsync(
				async () => await _financialPlanStatusService.GetOpenFinancialPlansStatuses(), 
				FinancialPlanOpenStatusKey);
		}

		private async Task<IList<FinancialPlanStatusDto>> GetFinancialPlansStatusesAsync(Func<Task<IList<FinancialPlanStatusDto>>> getFinancialPlanStatusesAsync, string cacheKey)
		{
			// Check cache
			var financialPlanStatuses = await GetData<IList<FinancialPlanStatusDto>>(cacheKey);
			if (financialPlanStatuses != null) return financialPlanStatuses;

			// Fetch from Academies API
			financialPlanStatuses = await getFinancialPlanStatusesAsync();

			// Save to cache
			await StoreData(cacheKey, financialPlanStatuses);
				
			return financialPlanStatuses;
		}
	}
}