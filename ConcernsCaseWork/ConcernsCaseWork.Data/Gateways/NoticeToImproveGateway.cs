using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Data.Gateways
{
	public class NoticeToImproveGateway : INoticeToImproveGateway
    {
        private readonly ConcernsDbContext _concernsDbContext;
        private readonly ILogger<NoticeToImproveGateway> _logger;

        public NoticeToImproveGateway(ConcernsDbContext concernsDbContext, ILogger<NoticeToImproveGateway> logger)
		{
            _concernsDbContext = concernsDbContext;
            _logger = logger;
        }

        public async Task<NoticeToImprove> CreateNoticeToImprove(NoticeToImprove request)
        {
            try
            {
                _concernsDbContext.NoticesToImprove.Add(request);
                await _concernsDbContext.SaveChangesAsync();
                return request;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError("Failed to create NoticeToImprove with Id {Id}, {ex}", request.Id, ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("An application exception has occurred whilst creating NoticeToImprove with Id {Id}, {ex}", request.Id, ex);
                throw;
            }
        }

        public async Task<NoticeToImprove> GetNoticeToImproveById(long noticeToImproveId)
        {
            try
            {
                return await _concernsDbContext.NoticesToImprove.Include(r => r.NoticeToImproveReasonsMapping).Include(c => c.NoticeToImproveConditionsMapping).SingleOrDefaultAsync(n => n.Id == noticeToImproveId);
            }
            catch (InvalidOperationException iox)
            {
                _logger.LogError(iox, "Multiple NoticeToImprove records found with Id: {Id}", noticeToImproveId);
                throw;
            }
        }

        public async Task<ICollection<NoticeToImprove>> GetNoticeToImproveByCaseUrn(int caseUrn)
        {
            return await _concernsDbContext.NoticesToImprove.Include(r => r.NoticeToImproveReasonsMapping).Include(c => c.NoticeToImproveConditionsMapping).Where(n => n.CaseUrn == caseUrn).ToListAsync();
        }

        public async Task<List<NoticeToImproveStatus>> GetAllStatuses()
        {
            return await _concernsDbContext.NoticeToImproveStatuses.ToListAsync();
        }

        public async Task<List<NoticeToImproveReason>> GetAllReasons()
        {
            return await _concernsDbContext.NoticeToImproveReasons.ToListAsync();
        }

        public async Task<List<NoticeToImproveConditionType>> GetAllConditionTypes()
        {
            return await _concernsDbContext.NoticeToImproveConditionTypes.ToListAsync();
        }

        public async Task<List<NoticeToImproveCondition>> GetAllConditions()
        {
            return await _concernsDbContext.NoticeToImproveConditions.Include(c => c.ConditionType).ToListAsync();
        }

        public async Task<NoticeToImprove> PatchNoticeToImprove(NoticeToImprove patchNoticeToImprove)
        {
            try
            {
                var reasonsToRemove = await _concernsDbContext.NoticeToImproveReasonsMappings.Where(r => r.NoticeToImproveId == patchNoticeToImprove.Id).ToListAsync();
                var conditionsToRemove = await _concernsDbContext.NoticeToImproveConditionsMappings.Where(r => r.NoticeToImproveId == patchNoticeToImprove.Id).ToListAsync();

                _concernsDbContext.NoticeToImproveReasonsMappings.RemoveRange(reasonsToRemove);
                _concernsDbContext.NoticeToImproveConditionsMappings.RemoveRange(conditionsToRemove);

                var tracked = _concernsDbContext.Update<NoticeToImprove>(patchNoticeToImprove);
                await _concernsDbContext.SaveChangesAsync();

                return tracked.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while trying to patch the NoticeToImprove with Id:", patchNoticeToImprove.Id);
                throw;
            }
        }
    }
}
