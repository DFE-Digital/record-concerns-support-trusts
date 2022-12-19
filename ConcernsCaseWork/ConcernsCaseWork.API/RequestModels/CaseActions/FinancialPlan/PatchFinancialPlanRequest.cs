using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.RequestModels.CaseActions.FinancialPlan
{
    public class PatchFinancialPlanRequest
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public int CaseUrn { get; set; }
        
        [StringLength(300)]
        public string Name { get; set; }
        public long? StatusId { get; set; }
        public DateTime? DatePlanRequested { get; set; }
        public DateTime? DateViablePlanReceived { get; set; }
        public DateTime CreatedAt { get; set; }
        
        [StringLength(300)]
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        
        [StringLength(2000)]
        public string Notes { get; set; }
    }
}