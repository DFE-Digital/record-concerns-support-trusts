using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class NtiModel : CaseActionModel
	{
		public NtiStatusModel Status { get; set; }
		public ICollection<NtiReasonModel> Reasons { get; set; }
		public ICollection<NtiConditionModel> Conditions { get; set; }
		public string Notes { get; set; }
		public DateTime? DateStarted { get; set; }
		public int? ClosedStatusId { get; set; }
		public NtiStatusModel ClosedStatus { get; set; }
		public string SumissionDecisionId { get; set; }
		public DateTime? DateNTILifted { get; set; }
		public DateTime? DateNTIClosed { get; set; }
	}
}
