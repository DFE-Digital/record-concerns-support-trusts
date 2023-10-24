using ConcernsCaseWork.API.Contracts.FinancialPlan;
using System;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class FinancialPlanModel : CaseActionModel
	{
		public DateTime? DatePlanRequested { get; }
		public DateTime? DateViablePlanReceived { get; }
		public string Notes { get; set; }
		public FinancialPlanStatus? Status { get; set; }

		public FinancialPlanModel()
		{
		}

		public FinancialPlanModel(long id, long caseUrn, DateTime createdAt, DateTime? datePlanRequested, DateTime? dateViablePlanReceived, 
			string notes, FinancialPlanStatus? status, DateTime? closedAt, DateTime updatedAt) =>
			(Id, CaseUrn, CreatedAt, DatePlanRequested, DateViablePlanReceived, Notes, Status, ClosedAt, UpdatedAt) =
			(id, caseUrn, createdAt, datePlanRequested, dateViablePlanReceived, notes, status, closedAt, updatedAt);
	}
}
