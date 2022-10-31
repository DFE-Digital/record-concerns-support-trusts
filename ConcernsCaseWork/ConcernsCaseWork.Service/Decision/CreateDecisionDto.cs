using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.Service.Decision
{ 
	public class CreateDecisionDto
	{
		[Range(1, int.MaxValue, ErrorMessage = "The ConcernsCaseUrn must be greater than zero")]
		public int ConcernsCaseUrn { get; set; }

		public DecisionType[] DecisionTypes { get; set; }

		[Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "The total amount requested must be zero or greater")]
		public decimal TotalAmountRequested { get; set; }

		[StringLength(MaxSupportingNotesLength, ErrorMessage = "Notes must be 2000 characters or less")]
		public string SupportingNotes { get; set; }

		public DateTimeOffset? ReceivedRequestDate { get; set; }

		[StringLength(MaxUrlLength, ErrorMessage = "Submission document link must be 2048 or less")]
		public string SubmissionDocumentLink { get; set; }
		public bool? SubmissionRequired { get; set; }
		public bool? RetrospectiveApproval { get; set; }

		[StringLength(MaxCaseNumberLength)]
		public string CrmCaseNumber { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset UpdatedAt { get; set; }


		public const int MaxUrlLength = 2048;
		public const int MaxSupportingNotesLength = 2000;
		public const int MaxCaseNumberLength = 20;
	}
}

