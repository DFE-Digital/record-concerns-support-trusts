using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Case
{
	public enum CaseType
	{
		[Description("A concern or multiple concerns")]
		Concerns = 1,
		[Description("Non-concern related action(s) or decision(s)")]
		NonConcerns = 2,
		ExConcerns = 3
	}
}
