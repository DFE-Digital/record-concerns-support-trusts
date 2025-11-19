using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Decisions
{
	public enum DrawdownFacilityAgreed
	{
		[Description("Yes")]
		Yes = 1,
		[Description("No")]
		No = 2,
		[Description("Payment under existing arrangement")]
		PaymentUnderExistingArrangement = 3,
		[Description("Final drawdown from this package")]
		FinalDrawdownFromThisPackage = 4
	}
}
