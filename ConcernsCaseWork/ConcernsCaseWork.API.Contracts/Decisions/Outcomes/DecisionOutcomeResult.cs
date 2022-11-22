using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.Contracts.Decisions.Outcomes
{
	public enum DecisionOutcomeResult
	{
		Approved = 1,
		ApprovedWithConditions = 2,
		PartiallyApproved = 3,
		Withdrawn = 4,
		Declined = 5
	}
}
