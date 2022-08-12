using Concerns.Data.Models;

namespace Concerns.Data.Gateways
{
    public interface IConcernsRecordGateway
    {
        ConcernsRecord SaveConcernsCase(ConcernsRecord concernsRecord);
        ConcernsRecord Update(ConcernsRecord concernsRecord);
        ConcernsRecord GetConcernsRecordByUrn(int urn);
    }
}