using ConcernsCaseWork.API.Contracts.NtiUnderConsideration;
using System.Collections.Generic;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class NtiUnderConsiderationModel : CaseActionModel
	{
		public ICollection<NtiUnderConsiderationReason> NtiReasonsForConsidering { get; set; }
		public string Notes { get; set; }
		public NtiUnderConsiderationClosedStatus? ClosedStatusId { get; set; }
		public string ClosedStatusName { get; set; }
	}
}