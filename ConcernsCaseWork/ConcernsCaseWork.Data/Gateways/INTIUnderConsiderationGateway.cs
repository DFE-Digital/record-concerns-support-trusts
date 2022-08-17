using Concerns.Data.Models;

namespace Concerns.Data.Gateways
{
    public interface INTIUnderConsiderationGateway
    {
        Task<NTIUnderConsideration> CreateNTIUnderConsideration(NTIUnderConsideration request);
        Task<ICollection<NTIUnderConsideration>> GetNTIUnderConsiderationByCaseUrn(int caseUrn);
        Task<NTIUnderConsideration> GetNTIUnderConsiderationById(long ntiUnderConsiderationId);
        Task<NTIUnderConsideration> PatchNTIUnderConsideration(NTIUnderConsideration patchNTIUnderConsideration);
        Task<List<NTIUnderConsiderationStatus>> GetAllStatuses();
        Task<List<NTIUnderConsiderationReason>> GetAllReasons();
    }
}
