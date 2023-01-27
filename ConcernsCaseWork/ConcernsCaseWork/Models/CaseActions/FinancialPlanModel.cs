using System;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class FinancialPlanModel : CaseActionModel
	{
		public DateTime? DatePlanRequested { get; }
		public DateTime? DateViablePlanReceived { get; }
		public string Notes { get; set; }
		public FinancialPlanStatusModel Status { get; set; }

		public FinancialPlanModel()
		{
			Status = new FinancialPlanStatusModel();
		}

		public FinancialPlanModel(long id, long caseUrn, DateTime createdAt, DateTime? datePlanRequested, DateTime? dateViablePlanReceived, 
			string notes, FinancialPlanStatusModel status, DateTime? closedAt, DateTime updatedAt) =>
			(Id, CaseUrn, CreatedAt, DatePlanRequested, DateViablePlanReceived, Notes, Status, ClosedAt, UpdatedAt) =
			(id, caseUrn, createdAt, datePlanRequested, dateViablePlanReceived, notes, status, closedAt, updatedAt);
	}
}
