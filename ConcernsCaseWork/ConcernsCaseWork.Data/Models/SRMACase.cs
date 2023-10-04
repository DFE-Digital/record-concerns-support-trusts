using ConcernsCaseWork.API.Contracts.Srma;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConcernsCaseWork.Data.Models
{
	public class SRMACase: IAuditable
    {
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
		public DateTime? DeletedAt { get; set; }


		// Entity framework will make this VARCHAR(MAX) over 4000, without specifying
		[Column(TypeName = "VARCHAR(5000)")]
		[StringLength(SrmaConstants.NotesLength)]
        public string Notes { get; set; }

        [ForeignKey(nameof(ReasonId))]
        public virtual SRMAReason Reason { get; set; }
    }
}