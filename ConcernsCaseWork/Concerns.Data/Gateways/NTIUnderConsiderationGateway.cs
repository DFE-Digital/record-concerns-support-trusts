using Concerns.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Concerns.Data.Gateways
{
	public class NTIUnderConsiderationGateway : INTIUnderConsiderationGateway
    {
        private readonly ConcernsDbContext _concernsDbContext;
        private readonly ILogger<NTIUnderConsiderationGateway> _logger;

        public NTIUnderConsiderationGateway(ConcernsDbContext concernsDbContext, ILogger<NTIUnderConsiderationGateway> logger)
		{
            _concernsDbContext = concernsDbContext;
            _logger = logger;
        }

        public async Task<NTIUnderConsideration> CreateNTIUnderConsideration(NTIUnderConsideration request)
        {
            try
            {
                _concernsDbContext.NTIUnderConsiderations.Add(request);
                await _concernsDbContext.SaveChangesAsync();
                return request;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError("Failed to create NTIUnderConsideration with Id {Id}, {ex}", request.Id, ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("An application exception has occurred whilst creating NTIUnderConsideration with Id {Id}, {ex}", request.Id, ex);
                throw;
            }
        }

        public async Task<ICollection<NTIUnderConsideration>> GetNTIUnderConsiderationByCaseUrn(int caseUrn)
        {
            return await _concernsDbContext.NTIUnderConsiderations.Include(r => r.UnderConsiderationReasonsMapping).Where(n => n.CaseUrn == caseUrn).ToListAsync();
        }

        public async Task<NTIUnderConsideration> GetNTIUnderConsiderationById(long ntiUnderConsiderationId)
        {
            try
            {
                return await _concernsDbContext.NTIUnderConsiderations.Include(r => r.UnderConsiderationReasonsMapping).SingleOrDefaultAsync(n => n.Id == ntiUnderConsiderationId);
            }
            catch (InvalidOperationException iox)
            {
                _logger.LogError(iox, "Multiple NTIUnderConsiderations records found with Id: {Id}", ntiUnderConsiderationId);
                throw;
            }
        }

        public async Task<NTIUnderConsideration> PatchNTIUnderConsideration(NTIUnderConsideration patchNTIUnderConsideration)
        {
            try
            {
                var reasonsToRemove = await _concernsDbContext.NTIUnderConsiderationReasonMappings.Where(r => r.NTIUnderConsiderationId == patchNTIUnderConsideration.Id).ToListAsync();
                _concernsDbContext.NTIUnderConsiderationReasonMappings.RemoveRange(reasonsToRemove);

                var tracked = _concernsDbContext.Update<NTIUnderConsideration>(patchNTIUnderConsideration);
                await _concernsDbContext.SaveChangesAsync();

                return tracked.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while trying to patch the NTI underconsideration with Id:", patchNTIUnderConsideration.Id);
                throw;
            }
        }

        public async Task<List<NTIUnderConsiderationStatus>> GetAllStatuses()
        {
            return await _concernsDbContext.NTIUnderConsiderationStatuses.ToListAsync();
        }

        public async Task<List<NTIUnderConsiderationReason>> GetAllReasons()
        {
            return await _concernsDbContext.NTIUnderConsiderationReasons.ToListAsync();
        }
    }
}

