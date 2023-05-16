using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Enums
{
    public enum FinancialPlanClosureStatus
	{
	    [Description("Viable Plan Received")]
		ViablePlanReceived = 3,

	    [Description("Abandoned")]
		Abandoned = 4,
    }
}
