using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.Contracts.Decisions.Outcomes
{
	public enum DecisionOutcomeStatus
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
