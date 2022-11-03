using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.Data.Gateways
{
    public class ConcernsTypeGateway : IConcernsTypeGateway
    {
        private readonly ConcernsDbContext _concernsDbContext;

        public ConcernsTypeGateway(ConcernsDbContext concernsDbContext)
        {
            _concernsDbContext = concernsDbContext;
        }

        public ConcernsType GetConcernsTypeById(int id)
        {
            return _concernsDbContext.ConcernsTypes.FirstOrDefault(t => t.Id == id);
        }

        public IList<ConcernsType> GetTypes()
        {
            return _concernsDbContext.ConcernsTypes.ToList();
        }
    }
}