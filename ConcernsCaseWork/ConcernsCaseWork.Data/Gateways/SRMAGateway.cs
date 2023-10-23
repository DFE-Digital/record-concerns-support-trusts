﻿using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Data.Gateways
{
	public interface ISRMAGateway
	{
		Task<SRMACase> CreateSRMA(SRMACase request);
		Task<ICollection<SRMACase>> GetSRMAsByCaseId(int caseId);
		Task<SRMACase> GetSRMAById(int srmaId);
		Task<SRMACase> PatchSRMAAsync(int srmaId, Func<SRMACase, SRMACase> patchDelegate);
	}

	public class SRMAGateway : ISRMAGateway
    {
        private readonly ConcernsDbContext _concernsDbContext;
        private readonly ILogger<SRMAGateway> _logger;

        public SRMAGateway(ConcernsDbContext concernsDbContext, ILogger<SRMAGateway> logger)
        {
            _concernsDbContext = concernsDbContext;
            _logger = logger;
        }

        public async Task<SRMACase> CreateSRMA(SRMACase request)
        {
            try
            {
	            request.UpdatedAt = request.CreatedAt;
                _concernsDbContext.SRMACases.Add(request);
                await _concernsDbContext.SaveChangesAsync();
                return request;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError("Failed to create SRMA with Id {Id}, {ex}", request.Id, ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("An application exception has occurred whilst creating SRMA with Id {Id}, {ex}", request.Id, ex);
                throw;
            }
        }

        public async Task<ICollection<SRMACase>> GetSRMAsByCaseId(int caseId)
        {
            return await _concernsDbContext.SRMACases.Where(s => s.CaseUrn == caseId).ToListAsync();
        }

        public async Task<SRMACase> GetSRMAById(int srmaId)
        {
            try
            {
                return await _concernsDbContext.SRMACases.SingleOrDefaultAsync(s => s.Id == srmaId);
            }
            catch (InvalidOperationException iox)
            {
                _logger.LogError(iox, "Multiple SRMA records found with Id: {Id}", srmaId);
                throw;
            }
        }

        public async Task<SRMACase> PatchSRMAAsync(int srmaId, Func<SRMACase, SRMACase> patchDelegate)
        {
            if (patchDelegate == null)
            {
                throw new ArgumentNullException("Delegate not provided");
            }

            try
            {
                var srma = await _concernsDbContext.SRMACases.FindAsync(srmaId);

                if (srma == null)
                {
                    throw new InvalidOperationException($"SRMA with Id:{srmaId} not found.");
                }

                var patchedSRMA = patchDelegate(srma);
                if (patchedSRMA == null || patchedSRMA.Id != srma.Id)
                {
                    throw new InvalidOperationException("Patched SRMA is invalid.");
                }

                patchedSRMA.UpdatedAt = DateTime.Now;
                
                var tracked = _concernsDbContext.Update<SRMACase>(patchedSRMA);
                await _concernsDbContext.SaveChangesAsync();

                return tracked.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while trying to patch the SRMA. SRMA Id:", srmaId);
                throw;
            }
        }
    }
}
