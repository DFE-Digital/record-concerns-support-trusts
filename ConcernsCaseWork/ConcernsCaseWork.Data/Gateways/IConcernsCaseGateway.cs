using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.Data.Gateways
{
    public interface IConcernsCaseGateway
    {
        ConcernsCase SaveConcernsCase(ConcernsCase concernsCase);
        IList<ConcernsCase> GetConcernsCaseByTrustUkprn(string trustUkprn, int page, int count);
        ConcernsCase GetConcernsCaseByUrn(int urn, bool withChangeTracking = false);
        ConcernsCase Update(ConcernsCase concernsCase);
        ConcernsCase GetConcernsCaseIncludingRecordsById(int id);
        IList<ConcernsCase> GetConcernsCasesByOwnerId(string ownerId, int? statusId, int page, int count);
        Task<ConcernsCase> UpdateExistingAsync(ConcernsCase concernsCase);
        Task<bool> CaseExists(int urn, CancellationToken cancellationToken = default);
        Task<string[]> GetOwnersOfOpenCases(CancellationToken cancellationToken = default);
    }
}