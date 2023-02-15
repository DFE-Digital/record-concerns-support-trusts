using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.FinancialPlans
{
	public enum FinancialPlanStatus
	{
		[Description("Viable Plan Received")]
		ViablePlanReceived = 3,
		[Description("Abandoned")]
		Abandoned = 4
	}
}
