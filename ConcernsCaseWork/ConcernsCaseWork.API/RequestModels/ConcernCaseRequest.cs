using ConcernsCaseWork.API.Contracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.RequestModels
{
    public class ConcernCaseRequest
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime ReviewAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string CreatedBy { get; set; }
        
        [MaxLength(0)] // not used
        public string Description { get; set; }
        
        [MaxLength(0)] // not used
        public string CrmEnquiry { get; set; }
        
        [MaxLength(50)]
        public string TrustUkprn { get; set; }
        
        [MaxLength(0)] // not used
        public string ReasonAtReview { get; set; }
        public DateTime? DeEscalation { get; set; }
        
        [MaxLength(2000)]
        public string Issue { get; set; }
        
        [MaxLength(4000)]
        public string CurrentStatus { get; set; }
        
        [MaxLength(1000)]
        public string CaseAim { get; set; }
        
        [MaxLength(1000)]
        public string DeEscalationPoint { get; set; }
        
        [MaxLength(4000)]
        public string NextSteps { get; set; }
        
        [MaxLength(4000)]
        public string CaseHistory { get; set; }
        
        [MaxLength(100)]
        public string DirectionOfTravel { get; set; }
        public int StatusId { get; set; }
        public int RatingId { get; set; }
        public TerritoryEnum? Territory { get; set; }
    }
}