using ConcernsCaseWork.Enums;
using System;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class FinancialPlanModel : CaseActionModel
	{
		public FinancialPlanStatus Status { get; set; }
		public DateTime? DatePlanRequested { get; set; }
		public DateTime? DateViablePlanReceived { get; set; }
		public string Notes { get; set; }

		public FinancialPlanModel(long id, long caseUrn, DateTime createdAt, FinancialPlanStatus status, DateTime? datePlanRequested, DateTime? dateViablePlanReceived, string notes) =>
			(Id, CaseUrn, CreatedAt, Status, DatePlanRequested, DateViablePlanReceived, Notes) =
			(id, caseUrn, createdAt, status, datePlanRequested, dateViablePlanReceived, notes);
	}
}
