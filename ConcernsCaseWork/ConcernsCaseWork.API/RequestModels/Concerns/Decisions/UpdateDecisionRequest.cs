using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConcernsCaseWork.Data.Enums.Concerns;
using DecisionType = ConcernsCaseWork.Data.Enums.Concerns.DecisionType;

namespace ConcernsCaseWork.API.RequestModels.Concerns.Decisions
{
	public class UpdateDecisionRequest
	{
		public DecisionType[] DecisionTypes { get; set; }

		[Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "The total amount requested must be zero or greater")]
		public decimal TotalAmountRequested { get; set; }

		[StringLength(Decision.MaxSupportingNotesLength)]
		public string SupportingNotes { get; set; }

		public DateTimeOffset ReceivedRequestDate { get; set; }

		[StringLength(Decision.MaxUrlLength)]
		public string SubmissionDocumentLink { get; set; }

		public bool? SubmissionRequired { get; set; }

		public bool? RetrospectiveApproval { get; set; }

		[StringLength(Decision.MaxCaseNumberLength)]
		public string CrmCaseNumber { get; set; }

		public bool IsValid()
		{
			return DecisionTypes.All(x => Enum.IsDefined(typeof(DecisionType), x));
		}
	}
}
