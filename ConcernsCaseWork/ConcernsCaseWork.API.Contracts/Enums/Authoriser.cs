using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Enums
{
	public enum Authoriser
	{
		[Description("G7")]
		G7 = 1,

		[Description("G6")]
		G6 = 2,

		[Description("Regional Schools Commisioner (RSC - Advisory board)")]
		RegionalSchoolsCommisioner = 3,

		[Description("Deputy Director")]
		DeputyDirector = 4,

		[Description("Countersigning Deputy Director")]
		CounterSigningDeputyDirector = 5,

		[Description("Director")]
		Director = 5,

		[Description("Minister")]
		Minister = 5
	}
}