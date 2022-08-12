using System;

namespace ConcernsCaseWork.Models.CaseActions
{
	public sealed class PatchFinancialPlanModel
	{
		public DateTime? ClosedAt { get; set; }
		public long Id { get; set; }
		public long CaseUrn { get; set; }
		public long? StatusId { get; set; }
		public DateTime? DatePlanRequested { get; set; }
		public DateTime? DateViablePlanReceived { get; set; }
		public string Notes { get; set; }
	}
}
