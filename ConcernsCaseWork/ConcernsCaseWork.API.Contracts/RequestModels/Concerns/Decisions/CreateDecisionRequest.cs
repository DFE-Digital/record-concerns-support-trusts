using ConcernsCaseWork.API.Contracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions
{
	public class CreateDecisionRequest
	{
		private const int _maxUrlLength = 2048;
		private const int _maxSupportingNotesLength = 2000;
		private const int _maxCaseNumberLength = 20;

		[Range(1, int.MaxValue, ErrorMessage = "The ConcernsCaseUrn must be greater than zero")]
		public int ConcernsCaseUrn { get; set; } // TODO: Remove this and pass urn separately to the decision request.

		public DecisionType[] DecisionTypes { get; set; }

		[Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "The total amount requested must be zero or greater")]
		public decimal TotalAmountRequested { get; set; }

		[StringLength(_maxSupportingNotesLength)]
		public string SupportingNotes { get; set; }

		public DateTimeOffset ReceivedRequestDate { get; set; }

		[StringLength(_maxUrlLength)]
		public string SubmissionDocumentLink { get; set; }

		public bool? SubmissionRequired { get; set; }

		public bool? RetrospectiveApproval { get; set; }

		[StringLength(_maxCaseNumberLength)]
		public string CrmCaseNumber { get; set; }

		public bool IsValid()
		{
			return DecisionTypes.All(x => Enum.IsDefined(typeof(DecisionType), x));
		}
	}
}