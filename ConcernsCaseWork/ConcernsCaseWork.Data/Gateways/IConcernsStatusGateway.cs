using Concerns.Data.Models;

namespace Concerns.Data.Gateways
{
    public interface IConcernsStatusGateway
    {
        IList<ConcernsStatus> GetStatuses();
        ConcernsStatus GetStatusByUrn(int urn);
    }
}