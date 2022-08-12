using Concerns.Data.Models;

namespace Concerns.Data.Gateways
{
    public class ConcernsTypeGateway : IConcernsTypeGateway
    {
        private readonly ConcernsDbContext _concernsDbContext;

        public ConcernsTypeGateway(ConcernsDbContext concernsDbContext)
        {
            _concernsDbContext = concernsDbContext;
        }

        public ConcernsType GetConcernsTypeByUrn(int urn)
        {
            return _concernsDbContext.ConcernsTypes.FirstOrDefault(t => t.Urn == urn);
        }

        public IList<ConcernsType> GetTypes()
        {
            return _concernsDbContext.ConcernsTypes.ToList();
        }
    }
}