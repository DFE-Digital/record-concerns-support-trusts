using ConcernsCaseWork.API.Contracts.Enums;

namespace ConcernsCaseWork.API.ResponseModels
{
    public class ConcernsCaseResponse
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime ReviewAt { get; set; }
        public DateTime ClosedAt { get; set; }
        public string CreatedBy { get; set; }
        public string Description { get; set; }
        public string CrmEnquiry { get; set; }
        public string TrustUkprn { get; set; }
        public string ReasonAtReview { get; set; }
        public DateTime? DeEscalation { get; set; }
        public string Issue { get; set; }
        public string CurrentStatus { get; set; }
        public string CaseAim { get; set; }
        public string DeEscalationPoint { get; set; }
        public string NextSteps { get; set; }
        public string CaseHistory { get; set; }
        public string DirectionOfTravel { get; set; }
        public int Urn { get; set; }
        public int StatusId { get; set; }
        public int RatingId { get; set; }
        public TerritoryEnum? Territory { get; set; }
    }
}