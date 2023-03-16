using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Decisions.Outcomes
{
	public enum DecisionOutcomeAuthorizer
	{
		[Description("Grade 7")]
		G7 = 1,

		[Description("Grade 6")]
		G6 = 2,

		[Description("Regional Director")]
		RegionalDirector = 3,

		[Description("Deputy Director")]
		DeputyDirector = 4,

		[Description("Countersigning Deputy Director")]
		CounterSigningDeputyDirector = 5,

		[Description("Director")]
		Director = 6,

		[Description("Minister")]
		Minister = 7
	}
}
