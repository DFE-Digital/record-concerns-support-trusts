using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Decisions
{
	public enum AcademyTrustPayApproval
	{
		[Description("Remuneration exceeding threshold")]
		RemunerationExceedingThreshold = 1,

		[Description("Performance-related pay exceeding threshold")]
		PerformanceRelatedExceedingThreshold = 2,

		[Description("Increase of executive pay at a faster rate")]
		IncreaseOfExecutivePayAtFasterRate = 3,
	}
}
