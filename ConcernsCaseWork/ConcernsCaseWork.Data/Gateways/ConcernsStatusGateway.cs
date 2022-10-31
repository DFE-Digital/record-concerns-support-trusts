using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.Data.Gateways
{
    public class ConcernsStatusGateway : IConcernsStatusGateway
    {
        private readonly ConcernsDbContext _concernsDbContext;
        
        public ConcernsStatusGateway(ConcernsDbContext concernsDbContext)
        {
            _concernsDbContext = concernsDbContext;
        }
        
        public IList<ConcernsStatus> GetStatuses()
        {
            return _concernsDbContext.ConcernsStatus.ToList();
        }

        public ConcernsStatus GetStatusByUrn(int urn)
        {
            return _concernsDbContext.ConcernsStatus.FirstOrDefault(s => s.Urn == urn);
        }
    }
}