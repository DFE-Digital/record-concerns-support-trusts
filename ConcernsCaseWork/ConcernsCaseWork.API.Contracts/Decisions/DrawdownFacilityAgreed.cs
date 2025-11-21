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
		FinalDrawdownFromThisPackage = 4,
		[Description("A new package with drawdown")]
		ANewPackageWithDrawdown = 5,
		[Description("A new package with immediate payment")]
		ANewPackageWithImmediatePayment = 6,
		[Description("A drawdown from an existing package")]
		ADrawdownFromAnExistingPackage = 7
	}
}
