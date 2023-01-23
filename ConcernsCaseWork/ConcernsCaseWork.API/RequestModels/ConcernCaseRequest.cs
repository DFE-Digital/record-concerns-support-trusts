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
        
        [StringLength(0)] // not used
        public string Description { get; set; }
        
        [StringLength(0)] // not used
        public string CrmEnquiry { get; set; }
        
        [StringLength(50)]
        public string TrustUkprn { get; set; }
        
        [StringLength(200)]
        public string ReasonAtReview { get; set; }
        public DateTime? DeEscalation { get; set; }
        
        [StringLength(2000)]
        public string Issue { get; set; }
        
        [StringLength(4000)]
        public string CurrentStatus { get; set; }
        
        [StringLength(1000)]
        public string CaseAim { get; set; }
        
        [StringLength(1000)]
        public string DeEscalationPoint { get; set; }
        
        [StringLength(4000)]
        public string NextSteps { get; set; }
        
        [StringLength(4300)]
        public string CaseHistory { get; set; }
        
        [StringLength(100)]
        public string DirectionOfTravel { get; set; }
        public int StatusId { get; set; }
        public int RatingId { get; set; }
        public Territory? Territory { get; set; }
    }
}