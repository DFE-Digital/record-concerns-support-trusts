using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.Data.Gateways
{
    public interface ISRMAGateway
    {
        Task<SRMACase> CreateSRMA(SRMACase request);
        Task<ICollection<SRMACase>> GetSRMAsByCaseId(int caseId);
        Task<SRMACase> GetSRMAById(int srmaId);
        Task<SRMACase> PatchSRMAAsync(int srmaId, Func<SRMACase, SRMACase> patchDelegate);
    }
}
