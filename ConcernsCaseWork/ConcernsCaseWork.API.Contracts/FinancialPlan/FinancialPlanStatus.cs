using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.FinancialPlan
{
	public enum FinancialPlanStatus
	{
		[Description("Awaiting plan")]
		AwaitingPlan = 1,

		[Description("Return to trust for further work")]
		ReturnToTrust = 2,

		[Description("Viable plan received")]
		ViablePlanReceived = 3,

		[Description("Abandoned")]
		Abandoned = 4
	}
}
