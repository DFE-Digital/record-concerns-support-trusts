using Concerns.Data.Models;
using Concerns.Data.ResponseModels;

namespace Concerns.Data.Factories
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
                Urn = concernsRecord.Urn,
                StatusUrn = concernsRecord.StatusUrn,
                TypeUrn = concernsRecord.ConcernsType.Urn,
                CaseUrn = concernsRecord.ConcernsCase.Urn,
                RatingUrn = concernsRecord.ConcernsRating.Urn,
                MeansOfReferralUrn = concernsRecord.ConcernsMeansOfReferral?.Urn
            };
        }
    }
}