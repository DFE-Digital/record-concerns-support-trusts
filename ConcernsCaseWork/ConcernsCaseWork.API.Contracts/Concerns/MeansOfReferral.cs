using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Concerns
{
	public enum MeansOfReferral
	{
		[Description("Internal department activity")]
		Internal = 1,

		[Description("An external source")]
		External = 2,

		[Description("Whistleblowing")]
		Whistleblowing = 3
	}
}
