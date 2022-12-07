using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Factories
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
                CaseHistory = concernsCase.CaseHistory,
                DirectionOfTravel = concernsCase.DirectionOfTravel,
                Urn = concernsCase.Urn,
                StatusId = concernsCase.StatusId,
                RatingId = concernsCase.RatingId,
                Territory = concernsCase.Territory
            };
        }
    }
}