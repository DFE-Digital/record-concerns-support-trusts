using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Enums
{
	public enum DecisionOutcome
	{
		[Description("Approved")]
		Approved = 1,

		[Description("Approved with conditions")]
		ApprovedWithConditions = 2,

		[Description("Partially approved")]
		PartiallyApproved = 3,

		[Description("Withdrawn")]
		Withdrawn = 4,

		[Description("Declined")]
		Declined = 5
	}
}