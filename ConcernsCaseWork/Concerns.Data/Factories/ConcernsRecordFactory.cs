using Concerns.Data.Models;
using Concerns.Data.RequestModels;

namespace Concerns.Data.Factories
{
    public static class ConcernsRecordFactory
    {
        public static ConcernsRecord Create(
            ConcernsRecordRequest concernsRecordRequest, 
            ConcernsCase concernsCase, 
            ConcernsType concernsType, 
            ConcernsRating concernsRating,
            ConcernsMeansOfReferral concernsMeansOfReferral = null)
        {
            return new ConcernsRecord
            {
                CreatedAt = concernsRecordRequest.CreatedAt,
                UpdatedAt = concernsRecordRequest.UpdatedAt,
                ReviewAt = concernsRecordRequest.ReviewAt,
                ClosedAt = concernsRecordRequest.ClosedAt,
                Name = concernsRecordRequest.Name,
                Description = concernsRecordRequest.Description,
                Reason = concernsRecordRequest.Reason,
                ConcernsCase = concernsCase,
                ConcernsType = concernsType,
                ConcernsRating = concernsRating,
                StatusUrn = concernsRecordRequest.StatusUrn,
                ConcernsMeansOfReferral = concernsMeansOfReferral
            };
        }

        public static ConcernsRecord Update( 
            ConcernsRecord original,
            ConcernsRecordRequest concernsRecordRequest, 
            ConcernsCase concernsCase, 
            ConcernsType concernsType, 
            ConcernsRating concernsRating, 
            ConcernsMeansOfReferral concernsMeansOfReferral = null)
        {
            original.CreatedAt = concernsRecordRequest.CreatedAt;
            original.UpdatedAt = concernsRecordRequest.UpdatedAt;
            original.ReviewAt = concernsRecordRequest.ReviewAt;
            original.ClosedAt = concernsRecordRequest.ClosedAt;
            original.Name = concernsRecordRequest.Name ?? original.Name;
            original.Description = concernsRecordRequest.Description ?? original.Name;
            original.Reason = concernsRecordRequest.Reason ?? original.Name;
            original.ConcernsCase = concernsCase;
            original.ConcernsType = concernsType;
            original.ConcernsRating = concernsRating;
            original.StatusUrn = concernsRecordRequest.StatusUrn;

            if (concernsMeansOfReferral != null)
            {
                original.ConcernsMeansOfReferral = concernsMeansOfReferral;
            }

            return original;
        }
    }
}