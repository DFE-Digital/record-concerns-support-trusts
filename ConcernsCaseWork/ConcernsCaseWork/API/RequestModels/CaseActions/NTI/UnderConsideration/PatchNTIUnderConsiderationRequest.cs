using System.ComponentModel.DataAnnotations;

namespace Concerns.Data.RequestModels.CaseActions.NTI.UnderConsideration
{
    public class PatchNTIUnderConsiderationRequest
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public int CaseUrn { get; set; }
        public string Notes { get; set; }

        public ICollection<int> UnderConsiderationReasonsMapping { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public int? ClosedStatusId { get; set; }
    }
}
