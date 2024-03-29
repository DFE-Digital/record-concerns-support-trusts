﻿using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Contracts.NtiUnderConsideration
{
    public class PatchNTIUnderConsiderationRequest
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public int CaseUrn { get; set; }
        
        [StringLength(2000)]
        public string Notes { get; set; }

        public ICollection<int> UnderConsiderationReasonsMapping { get; set; }

        [StringLength(300)]
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public int? ClosedStatusId { get; set; }
    }
}
