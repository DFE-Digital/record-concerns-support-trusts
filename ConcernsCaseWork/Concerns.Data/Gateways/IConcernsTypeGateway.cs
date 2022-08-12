using Concerns.Data.Models;

namespace Concerns.Data.Gateways
{
    public interface IConcernsTypeGateway
    {
        ConcernsType GetConcernsTypeByUrn(int urn);
        IList<ConcernsType> GetTypes();
    }
}