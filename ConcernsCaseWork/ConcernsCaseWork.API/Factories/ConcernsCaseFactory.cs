using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Factories
{
    public class ConcernsCaseFactory
    {
        public static ConcernsCase Create(ConcernCaseRequest request)
        {
            return new ConcernsCase
            {
                CreatedAt = request.CreatedAt,
                UpdatedAt = request.UpdatedAt,
                ReviewAt = request.ReviewAt,
                ClosedAt = request.ClosedAt,
                CreatedBy = request.CreatedBy,
                Description = request.Description,
                CrmEnquiry = request.CrmEnquiry,
                TrustUkprn = request.TrustUkprn,
                ReasonAtReview = request.ReasonAtReview,
                DeEscalation = request.DeEscalation,
                Issue = request.Issue,
                CurrentStatus = request.CurrentStatus,
                CaseAim = request.CaseAim,
                DeEscalationPoint = request.DeEscalationPoint,
                NextSteps = request.NextSteps,
                DirectionOfTravel = request.DirectionOfTravel,
                StatusId = request.StatusId,
                RatingId = request.RatingId,
            };
        }
        
        public static ConcernsCase Update(ConcernsCase original, ConcernCaseRequest updateRequest)
        {
            if (updateRequest == null)
            {
                return original;
            }
            
            var toMerge = Create(updateRequest);

            original.CreatedAt = toMerge.CreatedAt;
            original.UpdatedAt = toMerge.UpdatedAt;
            original.ReviewAt = toMerge.ReviewAt;
            original.ClosedAt = toMerge.ClosedAt;
            original.CreatedBy = toMerge.CreatedBy ?? original.CreatedBy;
            original.Description = toMerge.Description ?? original.Description;
            original.CrmEnquiry = toMerge.CrmEnquiry ?? original.CrmEnquiry;
            original.TrustUkprn = toMerge.TrustUkprn ?? original.TrustUkprn;
            original.ReasonAtReview = toMerge.ReasonAtReview ?? original.ReasonAtReview;
            original.DeEscalation = toMerge.DeEscalation ?? original.DeEscalation;
            original.Issue = toMerge.Issue ?? original.Issue;
            original.CurrentStatus = toMerge.CurrentStatus ?? original.CurrentStatus;
            original.CaseAim = toMerge.CaseAim ?? original.CaseAim;
            original.DeEscalationPoint = toMerge.DeEscalationPoint ?? original.DeEscalationPoint;
            original.NextSteps = toMerge.NextSteps ?? original.NextSteps;
            original.DirectionOfTravel = toMerge.DirectionOfTravel ?? original.DirectionOfTravel;
            original.StatusId = toMerge.StatusId;
            original.RatingId = toMerge.RatingId;

            return original;
        }
    }
}