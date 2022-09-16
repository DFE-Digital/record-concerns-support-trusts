using ConcernsCaseWork.Data.Models;

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

        public ConcernsRecord GetConcernsRecordByUrn(int urn)
        {
            return _concernsDbContext.ConcernsRecord.FirstOrDefault(r => r.Urn == urn);
        }
    }
}