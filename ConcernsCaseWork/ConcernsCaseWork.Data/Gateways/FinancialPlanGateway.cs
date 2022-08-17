using Concerns.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Concerns.Data.Gateways
{
    public class FinancialPlanGateway : IFinancialPlanGateway
    {
        private readonly ConcernsDbContext _concernsDbContext;
        private readonly ILogger<SRMAGateway> _logger;

        public FinancialPlanGateway(ConcernsDbContext concernsDbContext, ILogger<SRMAGateway> logger)
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
