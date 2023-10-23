using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.FinancialPlan
{
	public enum FinancialPlanClosureStatus
	{
		[Description("Viable Plan Received")]
		ViablePlanReceived = 3,

		[Description("Abandoned")]
		Abandoned = 4,
	}
}
