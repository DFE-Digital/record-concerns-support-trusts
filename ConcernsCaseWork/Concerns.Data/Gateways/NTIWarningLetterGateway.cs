using Concerns.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Concerns.Data.Gateways
{
	public class NTIWarningLetterGateway : INTIWarningLetterGateway
    {
        private readonly ConcernsDbContext _concernsDbContext;
        private readonly ILogger<NTIWarningLetterGateway> _logger;

        public NTIWarningLetterGateway(ConcernsDbContext concernsDbContext, ILogger<NTIWarningLetterGateway> logger)
		{
            _concernsDbContext = concernsDbContext;
            _logger = logger;
        }

        public async Task<NTIWarningLetter> CreateNTIWarningLetter(NTIWarningLetter request)
        {
            try
            {
                _concernsDbContext.NTIWarningLetters.Add(request);
                await _concernsDbContext.SaveChangesAsync();
                return request;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError("Failed to create NTIWarningLetter with Id {Id}, {ex}", request.Id, ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("An application exception has occurred whilst creating NTIWarningLetter with Id {Id}, {ex}", request.Id, ex);
                throw;
            }
        }

        public async Task<List<NTIWarningLetterCondition>> GetAllConditions()
        {
            return await _concernsDbContext.NTIWarningLetterConditions.Include(c => c.ConditionType).ToListAsync();
        }

        public async Task<List<NTIWarningLetterConditionType>> GetAllConditionTypes()
        {
            return await _concernsDbContext.NTIWarningLetterConditionTypes.ToListAsync();
        }

        public async Task<List<NTIWarningLetterReason>> GetAllReasons()
        {
            return await _concernsDbContext.NTIWarningLetterReasons.ToListAsync();
        }

        public async Task<List<NTIWarningLetterStatus>> GetAllStatuses()
        {
            return await _concernsDbContext.NTIWarningLetterStatuses.ToListAsync();
        }

        public async Task<NTIWarningLetter> GetNTIWarningLetterById(long ntiWarningLetterId)
        {
            try
            {
                return await _concernsDbContext.NTIWarningLetters.Include(r => r.WarningLetterReasonsMapping).Include(c => c.WarningLetterConditionsMapping).SingleOrDefaultAsync(n => n.Id == ntiWarningLetterId);
            }
            catch (InvalidOperationException iox)
            {
                _logger.LogError(iox, "Multiple NTIWarningLetter records found with Id: {Id}", ntiWarningLetterId);
                throw;
            }
        }

        public async Task<ICollection<NTIWarningLetter>> GetNTIWarningLetterByCaseUrn(int caseUrn)
        {
            return await _concernsDbContext.NTIWarningLetters.Include(r => r.WarningLetterReasonsMapping).Include(c => c.WarningLetterConditionsMapping).Where(n => n.CaseUrn == caseUrn).ToListAsync();
        }

        public async Task<NTIWarningLetter> PatchNTIWarningLetter(NTIWarningLetter patchNTIWarningLetter)
        {
            try
            {
                var reasonsToRemove = await _concernsDbContext.NTIWarningLetterReasonsMapping.Where(r => r.NTIWarningLetterId == patchNTIWarningLetter.Id).ToListAsync();
                var conditionsToRemove = await _concernsDbContext.NTIWarningLetterConditionsMapping.Where(r => r.NTIWarningLetterId == patchNTIWarningLetter.Id).ToListAsync();

                _concernsDbContext.NTIWarningLetterReasonsMapping.RemoveRange(reasonsToRemove);
                _concernsDbContext.NTIWarningLetterConditionsMapping.RemoveRange(conditionsToRemove);

                var tracked = _concernsDbContext.Update<NTIWarningLetter>(patchNTIWarningLetter);
                await _concernsDbContext.SaveChangesAsync();

                return tracked.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while trying to patch the NTIWarningLetter with Id:", patchNTIWarningLetter.Id);
                throw;
            }
        }
    }
}

