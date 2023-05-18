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
	        try
	        {
				_concernsDbContext.ConcernsCase.Update(concernsCase);
				_concernsDbContext.SaveChanges();
	        }
	        catch (Exception e)
	        {
		       
		        throw;
	        }
            

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

        public ConcernsCase GetConcernsCaseByUrn(int urn, bool withChangeTracking = false)
        {
			var exp = _concernsDbContext.ConcernsCase
				.Include(x => x.Decisions)
				.ThenInclude(x => x.DecisionTypes)
				.Include(x => x.Decisions)
				.ThenInclude(x => x.Outcome)
				.ThenInclude(x => x.BusinessAreasConsulted);

			if (!withChangeTracking)
	        {
		        exp.AsNoTracking();
	        }

	        var concernsCase = exp.FirstOrDefault(c => c.Urn == urn);

	        return concernsCase;
        }

        public ConcernsCase GetConcernsCaseIncludingRecordsById(int id)
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
                .FirstOrDefault(c => c.Urn == id);

            return concernsCase;
        }

        public IList<ConcernsCase> GetConcernsCasesByOwnerId(string ownerId, int? statusId, int page, int count)
        {
            var query = _concernsDbContext.ConcernsCase
                .Include(x => x.Decisions)
                .ThenInclude(x => x.DecisionTypes)
                .Where(c => c.CreatedBy == ownerId);

            if (statusId != null)
            {
                query = query.Where(c => c.StatusId == statusId);
            }

            query = query.Skip((page - 1) * count)
                .Take(count)
                .AsNoTracking();

            return query.ToList();
        }

        public async Task<ConcernsCase> UpdateExistingAsync(ConcernsCase concernsCase)
        {
	        await _concernsDbContext.SaveChangesAsync();
	        return concernsCase;
        }

        public ConcernsCase Update(ConcernsCase concernsCase)
        {
            var entity = _concernsDbContext.ConcernsCase.Update(concernsCase);
            _concernsDbContext.SaveChanges();
            return entity.Entity;
        }
        
        public async Task<bool> CaseExists(int urn, CancellationToken cancellationToken = default) 
	        => await _concernsDbContext.ConcernsCase.AnyAsync(c => c.Urn == urn, cancellationToken);

        public async Task<string[]> GetOwnersOfOpenCases(CancellationToken cancellationToken = default)
        {
	        return await _concernsDbContext.ConcernsCase.Where(x => x.StatusId == 1 || x.StatusId == 2)
		        .Select(x => x.CreatedBy)
		        .Distinct()
		        .ToArrayAsync(cancellationToken);
        }
    }
}