using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.Data.Gateways
{
    public interface IConcernsTypeGateway
    {
        ConcernsType GetConcernsTypeByUrn(int urn);
        IList<ConcernsType> GetTypes();
    }
}