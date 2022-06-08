using ConcernsCaseWork.Enums;
using System;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class FinancialPlanModel : CaseActionModel
	{
		public DateTime? DatePlanRequested { get; set; }
		public DateTime? DateViablePlanReceived { get; set; }
		public string Notes { get; set; }
		public FinancialPlanStatusModel Status { get; set; }

		public FinancialPlanModel(long id, long caseUrn, DateTime createdAt, DateTime? datePlanRequested, DateTime? dateViablePlanReceived, string notes, FinancialPlanStatusModel status, DateTime? closedAt) =>
			(Id, CaseUrn, CreatedAt, DatePlanRequested, DateViablePlanReceived, Notes, Status, ClosedAt) =
			(id, caseUrn, createdAt, datePlanRequested, dateViablePlanReceived, notes, status, closedAt);
	}
}
