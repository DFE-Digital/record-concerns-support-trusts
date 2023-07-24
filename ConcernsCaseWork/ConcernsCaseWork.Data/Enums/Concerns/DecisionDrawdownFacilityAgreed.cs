using System.ComponentModel;

namespace ConcernsCaseWork.Data.Enums.Concerns
{
    public enum DecisionDrawdownFacilityAgreed
	{
		[Description("Yes")]
		Yes = 1,

		[Description("No")]
		No = 2,

		[Description("Payment Under Existing Arrangement")]
		PaymentUnderExistingArrangement = 3,
	}
}
