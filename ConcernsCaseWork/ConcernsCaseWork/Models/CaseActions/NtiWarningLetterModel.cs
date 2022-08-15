using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class NtiWarningLetterModel : CaseActionModel
	{
		public NtiWarningLetterStatusModel Status { get; set; }
		public ICollection<NtiWarningLetterReasonModel> Reasons { get; set; }
		public ICollection<NtiWarningLetterConditionModel> Conditions { get; set; }
		public string Notes { get; set; }
		public DateTime? SentDate { get; set; }
		public int? ClosedStatusId { get; set; }
		public NtiWarningLetterStatusModel ClosedStatus { get; set; }
	}
}