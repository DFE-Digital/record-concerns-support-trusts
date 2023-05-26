using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Gateways
{
    public class ConcernsRecordGateway : IConcernsRecordGateway
    {
        private readonly ConcernsDbContext _concernsDbContext;

        public ConcernsRecordGateway(ConcernsDbContext concernsDbContext)
        {
            _concernsDbContext = concernsDbContext;
        }

        public ConcernsRecord SaveConcernsCase(ConcernsRecord concernsRecord)
        {
            _concernsDbContext.ConcernsRecord.Update(concernsRecord);
            _concernsDbContext.SaveChanges();

            return concernsRecord;
        }

        public ConcernsRecord Update(ConcernsRecord concernsRecord)
        {
            var entity = _concernsDbContext.ConcernsRecord.Update(concernsRecord);
            _concernsDbContext.SaveChanges();
            return entity.Entity;
        }

        public ConcernsRecord GetConcernsRecordByUrn(int id)
        {
            return _concernsDbContext.ConcernsRecord
				.Include(f=> f.ConcernsType)
				.Include(f => f.ConcernsCase)
				.Include(f => f.ConcernsRating)
				.Include(f => f.ConcernsMeansOfReferral)
				.SingleOrDefault(r => r.Id == id);
        }

		public void Delete(int id)
		{
			var c = _concernsDbContext.ConcernsRecord.SingleOrDefault(f=> f.Id == id);
			c.DeletedAt = DateTime.Now;
			_concernsDbContext.SaveChanges();
		}
	}
}