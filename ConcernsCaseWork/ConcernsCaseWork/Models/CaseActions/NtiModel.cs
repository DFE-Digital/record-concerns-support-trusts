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
		public DateTime? SentDate { get; set; }
		public int? ClosedStatusId { get; set; }
		public NtiStatusModel ClosedStatus { get; set; }
	}
}
