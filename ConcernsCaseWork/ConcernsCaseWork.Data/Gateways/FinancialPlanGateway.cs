﻿using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Data.Gateways
{
	public interface IFinancialPlanGateway
	{
		Task<FinancialPlanCase> CreateFinancialPlan(FinancialPlanCase request);
		Task<FinancialPlanCase> GetFinancialPlanById(long financialPlanId);
		Task<ICollection<FinancialPlanCase>> GetFinancialPlansByCaseUrn(int caseUrn);
		Task<FinancialPlanCase> PatchFinancialPlan(FinancialPlanCase updatedFinancialPlan);
		Task<List<FinancialPlanStatus>> GetAllStatuses();
	}

	public class FinancialPlanGateway : IFinancialPlanGateway
    {
        private readonly ConcernsDbContext _concernsDbContext;
        private readonly ILogger<FinancialPlanGateway> _logger;

        public FinancialPlanGateway(ConcernsDbContext concernsDbContext, ILogger<FinancialPlanGateway> logger)
        {
            _concernsDbContext = concernsDbContext;
            _logger = logger;
        }

        public async Task<FinancialPlanCase> CreateFinancialPlan(FinancialPlanCase request)
        {
            try
            {
                var tracked = _concernsDbContext.FinancialPlanCases.Add(request);
                await _concernsDbContext.SaveChangesAsync();
                return tracked.Entity;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError("Failed to create Financial Plan with Id {Id}, {ex}", request.Id, ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("An application exception has occurred whilst creating Financial Plan with Id {Id}, {ex}", request.Id, ex);
                throw;
            }
        }

        public async Task<FinancialPlanCase> GetFinancialPlanById(long financialPlanId)
        {
            return await _concernsDbContext.FinancialPlanCases
                .Include(fp => fp.Status)
                .SingleOrDefaultAsync(fp => fp.Id == financialPlanId);
        }

        public async Task<ICollection<FinancialPlanCase>> GetFinancialPlansByCaseUrn(int caseUrn)
        {
            return await _concernsDbContext.FinancialPlanCases
                .Include(fp => fp.Status)
                .Where(s => s.CaseUrn == caseUrn).ToListAsync();
        }

        public async Task<FinancialPlanCase> PatchFinancialPlan(FinancialPlanCase patchedFinancialPlan)
        {
            try
            {
                var tracked = _concernsDbContext.Update<FinancialPlanCase>(patchedFinancialPlan);
                await _concernsDbContext.SaveChangesAsync();

                return tracked.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while trying to patch the financial plan with Id:", patchedFinancialPlan.Id);
                throw;
            }
        }

        public async Task<List<FinancialPlanStatus>> GetAllStatuses()
        {
            return await _concernsDbContext.FinancialPlanStatuses.ToListAsync();
        }
    }
}
