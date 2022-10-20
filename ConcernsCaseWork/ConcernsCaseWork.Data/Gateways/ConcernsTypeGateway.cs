using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.Data.Gateways
{
    public class ConcernsTypeGateway : IConcernsTypeGateway
    {
        private readonly ConcernsDbContext _concernsDbContext;

        public ConcernsTypeGateway(ConcernsDbContext tramsDbContext)
        {
            _concernsDbContext = tramsDbContext;
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