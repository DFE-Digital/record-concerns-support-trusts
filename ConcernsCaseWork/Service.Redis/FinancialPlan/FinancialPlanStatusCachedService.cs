using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using ConcernsCasework.Service.FinancialPlan;
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
			_logger.LogInformation("FinancialPlanStatusCachedService::GetAllFinancialPlanStatusesAsync");
			
			return await GetFinancialPlansStatusesAsync(
				async () => await _financialPlanStatusService.GetAllFinancialPlansStatusesAsync(), 
				FinancialPlanStatusKey);
		}

		public async Task<IList<FinancialPlanStatusDto>> GetClosureFinancialPlansStatusesAsync()
		{
			_logger.LogInformation("FinancialPlanStatusCachedService::GetClosureFinancialPlansStatusesAsync");
			
			return await GetFinancialPlansStatusesAsync(
				async () => await _financialPlanStatusService.GetClosureFinancialPlansStatusesAsync(), 
				FinancialPlanClosureStatusKey);
		}

		public async Task<IList<FinancialPlanStatusDto>> GetOpenFinancialPlansStatusesAsync()
		{
			_logger.LogInformation("FinancialPlanStatusCachedService::GetOpenFinancialPlansStatusesAsync");
			
			return await GetFinancialPlansStatusesAsync(
				async () => await _financialPlanStatusService.GetOpenFinancialPlansStatusesAsync(), 
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