using System;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class CaseActionModel
	{
		public long CaseUrn { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? ClosedAt { get; set; }
	}
}
