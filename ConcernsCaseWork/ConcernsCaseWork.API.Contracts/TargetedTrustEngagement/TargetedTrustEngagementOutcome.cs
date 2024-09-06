using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.TargetedTrustEngagement
{
	public enum TargetedTrustEngagementOutcome
	{
		[Description("Adequate response received")]
		AdequateResponseReceived = 1,

		[Description("Inadequate response received")]
		InadequateResponseReceived = 2,

		[Description("No engagement took place")]
		NoEngagementTookPlace = 3,

		[Description("No response required")]
		NoResponseRequired = 4
	}
}