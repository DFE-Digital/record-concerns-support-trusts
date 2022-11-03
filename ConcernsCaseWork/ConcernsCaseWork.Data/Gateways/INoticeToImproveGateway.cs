using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.Data.Gateways
{
    public interface INoticeToImproveGateway
    {
        Task<NoticeToImprove> CreateNoticeToImprove(NoticeToImprove request);
        Task<NoticeToImprove> GetNoticeToImproveById(long noticeToImproveId);
        Task<ICollection<NoticeToImprove>> GetNoticeToImproveByCaseUrn(int caseUrn);
        Task<List<NoticeToImproveStatus>> GetAllStatuses();
        Task<List<NoticeToImproveReason>> GetAllReasons();
        Task<List<NoticeToImproveConditionType>> GetAllConditionTypes();
        Task<List<NoticeToImproveCondition>> GetAllConditions();        
        Task<NoticeToImprove> PatchNoticeToImprove(NoticeToImprove patchNoticeToImprove);
    }
}
