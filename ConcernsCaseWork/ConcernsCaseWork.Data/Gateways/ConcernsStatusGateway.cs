using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.Data.Gateways
{
	public interface IConcernsStatusGateway
	{
		IList<ConcernsStatus> GetStatuses();
		ConcernsStatus GetStatusByUrn(int urn);
	}

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

        public ConcernsStatus GetStatusByUrn(int id)
        {
            return _concernsDbContext.ConcernsStatus.FirstOrDefault(s => s.Id == id);
        }
    }
}