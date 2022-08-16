using Concerns.Data.Models;
using Concerns.Data.ResponseModels;

namespace Concerns.Data.Factories
{
    public class ConcernsCaseResponseFactory
    {
        public static ConcernsCaseResponse Create(ConcernsCase concernsCase)
        {
            return new ConcernsCaseResponse
            {
                CreatedAt = concernsCase.CreatedAt,
                UpdatedAt = concernsCase.UpdatedAt,
                ReviewAt = concernsCase.ReviewAt,
                ClosedAt = concernsCase.ClosedAt,
                CreatedBy = concernsCase.CreatedBy,
                Description = concernsCase.Description,
                CrmEnquiry = concernsCase.CrmEnquiry,
                TrustUkprn = concernsCase.TrustUkprn,
                ReasonAtReview = concernsCase.ReasonAtReview,
                DeEscalation = concernsCase.DeEscalation,
                Issue = concernsCase.Issue,
                CurrentStatus = concernsCase.CurrentStatus,
                CaseAim = concernsCase.CaseAim,
                DeEscalationPoint = concernsCase.DeEscalationPoint,
                NextSteps = concernsCase.NextSteps,
                DirectionOfTravel = concernsCase.DirectionOfTravel,
                Urn = concernsCase.Urn,
                StatusUrn = concernsCase.StatusUrn,
                RatingUrn = concernsCase.RatingUrn
            };
        }
    }
}