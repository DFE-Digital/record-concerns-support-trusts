using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.Data.Gateways
{
    [Obsolete("This is planned to be moved into the Concerns Casework API. If it is accessed by other APIs, please let the Concerns team know.")]
    public interface ISRMAGateway
    {
        Task<SRMACase> CreateSRMA(SRMACase request);
        Task<ICollection<SRMACase>> GetSRMAsByCaseId(int caseId);
        Task<SRMACase> GetSRMAById(int srmaId);
        Task<SRMACase> PatchSRMAAsync(int srmaId, Func<SRMACase, SRMACase> patchDelegate);
    }
}
