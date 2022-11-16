using ConcernsCaseWork.API.Contracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions
{
	public class UpdateDecisionRequest
	{
		private const int _maxUrlLength = 2048;
		private const int _maxSupportingNotesLength = 2000;
		private const int _maxCaseNumberLength = 20;

		public DecisionType[] DecisionTypes { get; set; }

		[Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "The total amount requested must be zero or greater")]
		public decimal TotalAmountRequested { get; set; }

		[StringLength(_maxSupportingNotesLength, ErrorMessage  = "Notes must be 2000 characters or less", MinimumLength = 0)]
		public string SupportingNotes { get; set; }

		// TODO: This should potentially be optional. Waiting for Ben to clarify.
		public DateTimeOffset? ReceivedRequestDate { get; set; }

		[StringLength(_maxUrlLength, ErrorMessage = "Submission document link must be 2048 or less", MinimumLength = 0)]
		public string SubmissionDocumentLink { get; set; }

		public bool? SubmissionRequired { get; set; }

		public bool? RetrospectiveApproval { get; set; }

		[StringLength(_maxCaseNumberLength, MinimumLength = 0)]
		public string CrmCaseNumber { get; set; }

		public bool IsValid()
		{
			return DecisionTypes.All(x => Enum.IsDefined(typeof(DecisionType), x));
		}
	}
}
