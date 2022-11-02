using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.Data.Gateways
{
    public interface IConcernsStatusGateway
    {
        IList<ConcernsStatus> GetStatuses();
        ConcernsStatus GetStatusByUrn(int urn);
    }
}