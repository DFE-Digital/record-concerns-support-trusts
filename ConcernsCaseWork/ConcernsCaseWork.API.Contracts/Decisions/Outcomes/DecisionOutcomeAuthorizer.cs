using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.Contracts.Decisions.Outcomes
{
	public enum DecisionOutcomeAuthorizer
	{
		[Description("G7")]
		G7 = 1,

		[Description("G6")]
		G6 = 2,

		[Description("Regional Director (RD)")]
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
