using ConcernsCaseWork.API.Contracts.NoticeToImprove;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class NtiModel : CaseActionModel
	{
		public NtiStatus? Status { get; set; }
		public ICollection<NtiReason> Reasons { get; set; }
		public ICollection<NtiConditionModel> Conditions { get; set; }
		public string Notes { get; set; }
		public DateTime? DateStarted { get; set; }
		public NtiStatus? ClosedStatusId { get; set; }
		public string SubmissionDecisionId { get; set; }
		public DateTime? DateNTILifted { get; set; }
		public DateTime? DateNTIClosed { get; set; }
	}
}
