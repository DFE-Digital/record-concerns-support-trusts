using System.ComponentModel;

namespace ConcernsCaseWork.Enums
{
	public enum FinancialPlanStatus
	{
		Unknown,
		
		[Description("Awaiting plan")]
		AwaitingPlan,

		[Description("Return to trust for further work")]
		ReturnToTrust,

		[Description("Viable plan received")]
		ViablePlanReceived,

		[Description("Abandoned")]
		Abandoned
	}
}
