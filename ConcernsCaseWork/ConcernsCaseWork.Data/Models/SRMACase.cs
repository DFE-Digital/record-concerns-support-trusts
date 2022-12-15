﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcernsCaseWork.Data.Models
{
    [Table("SRMACase", Schema = "concerns")]
    public class SRMACase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CaseUrn { get; set; }
        public int StatusId { get; set; }
        public int? CloseStatusId { get; set; }
        public int? ReasonId { get; set; }
        public DateTime DateOffered { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DateReportSentToTrust { get; set; }
        public DateTime? DateAccepted { get; set; }
        public DateTime? StartDateOfVisit { get; set; }
        public DateTime? EndDateOfVisit { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string CreatedBy { get; set; }

        [StringLength(2000)]
        public string Notes { get; set; }

        [ForeignKey(nameof(ReasonId))]
        public virtual SRMAReason Reason { get; set; }
    }
}