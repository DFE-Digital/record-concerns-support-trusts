using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.Data.Gateways
{
    public interface IConcernsTypeGateway
    {
        ConcernsType GetConcernsTypeById(int urn);
        IList<ConcernsType> GetTypes();
    }
}