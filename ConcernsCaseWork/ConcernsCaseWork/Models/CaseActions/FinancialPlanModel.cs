using System;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class FinancialPlanModel : CaseActionModel
	{
		public DateTime? DatePlanRequested { get; }
		public DateTime? DateViablePlanReceived { get; }
		public string Notes { get; }
		public FinancialPlanStatusModel Status { get; }

		public FinancialPlanModel(long id, long caseUrn, DateTime createdAt, DateTime? datePlanRequested, DateTime? dateViablePlanReceived, string notes, FinancialPlanStatusModel status, DateTime? closedAt) =>
			(Id, CaseUrn, CreatedAt, DatePlanRequested, DateViablePlanReceived, Notes, Status, ClosedAt) =
			(id, caseUrn, createdAt, datePlanRequested, dateViablePlanReceived, notes, status, closedAt);
	}
}
