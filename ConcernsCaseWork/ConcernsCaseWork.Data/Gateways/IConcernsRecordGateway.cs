using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.Data.Gateways
{
    public interface IConcernsRecordGateway
    {
        ConcernsRecord SaveConcernsCase(ConcernsRecord concernsRecord);
        ConcernsRecord Update(ConcernsRecord concernsRecord);
        ConcernsRecord GetConcernsRecordByUrn(int urn);
    }
}