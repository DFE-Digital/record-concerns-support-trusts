using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Gateways
{
    public class ConcernsCaseGateway : IConcernsCaseGateway
    {
        private readonly ConcernsDbContext _concernsDbContext;
        
        public ConcernsCaseGateway(ConcernsDbContext concernsDbContext)
        {
            _concernsDbContext = concernsDbContext;
        }

        public ConcernsCase SaveConcernsCase(ConcernsCase concernsCase)
        {
            _concernsDbContext.ConcernsCase.Update(concernsCase);
            _concernsDbContext.SaveChanges();

            return concernsCase;
        }

        public IList<ConcernsCase> GetConcernsCaseByTrustUkprn(string trustUkPrn, int page, int count)
        {
            return _concernsDbContext.ConcernsCase
                .Where(c => c.TrustUkprn == trustUkPrn)
                .Skip((page - 1) * count)
                .Take(count)
                .Include(x => x.Decisions)
                .ThenInclude(x => x.DecisionTypes)
                .AsNoTracking()
                .ToList();
        }

        public ConcernsCase GetConcernsCaseByUrn(int urn)
        {
            var concernsCase = _concernsDbContext.ConcernsCase
                .Include(x => x.Decisions)
                .ThenInclude(x => x.DecisionTypes)
                .AsNoTracking()
                .FirstOrDefault(c => c.Urn == urn);
            
            return concernsCase;
        }
        
        public ConcernsCase GetConcernsCaseIncludingRecordsByUrn(int urn)
        {
            var concernsCase = _concernsDbContext.ConcernsCase
                .Include(c => c.ConcernsRecords)
                .ThenInclude(record => record.ConcernsRating)
                .Include(c => c.ConcernsRecords)
                .ThenInclude(record => record.ConcernsType)
                .Include(c => c.ConcernsRecords)
                .ThenInclude(record => record.ConcernsMeansOfReferral)
                .Include(x => x.Decisions)
                .ThenInclude(x => x.DecisionTypes)
                .AsNoTracking()
                .FirstOrDefault(c => c.Urn == urn);
            
            return concernsCase;
        }

        public IList<ConcernsCase> GetConcernsCasesByOwnerId(string ownerId, int? statusUrn, int page, int count)
        {

            var query = _concernsDbContext.ConcernsCase
                .Include(x => x.Decisions)
                .ThenInclude(x => x.DecisionTypes)
                .Where(c => c.CreatedBy == ownerId);

            if (statusUrn != null)
            {
                query = query.Where(c => c.StatusUrn == statusUrn);
            }

            query = query.Skip((page - 1) * count)
                .Take(count)
                .AsNoTracking();
            
            return query.ToList();
        }

        public ConcernsCase Update(ConcernsCase concernsCase)
        {
            var entity = _concernsDbContext.ConcernsCase.Update(concernsCase);
            _concernsDbContext.SaveChanges();
            return entity.Entity;
        }
    }
}