using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Factories
{
    public class ConcernsRecordResponseFactory
    {
        public static ConcernsRecordResponse Create(ConcernsRecord concernsRecord)
        {
            return new ConcernsRecordResponse
            {
                CreatedAt = concernsRecord.CreatedAt,
                UpdatedAt = concernsRecord.UpdatedAt,
                ReviewAt = concernsRecord.ReviewAt,
                ClosedAt = concernsRecord.ClosedAt,
                Name = concernsRecord.Name,
                Description = concernsRecord.Description,
                Reason = concernsRecord.Reason,
                Id = concernsRecord.Id,
                StatusId = concernsRecord.StatusId,
                TypeId = concernsRecord.ConcernsType.Id,
                CaseUrn = concernsRecord.ConcernsCase.Urn,
                RatingId = concernsRecord.ConcernsRating.Id,
                MeansOfReferralId = concernsRecord.ConcernsMeansOfReferral?.Id
            };
        }
    }
}