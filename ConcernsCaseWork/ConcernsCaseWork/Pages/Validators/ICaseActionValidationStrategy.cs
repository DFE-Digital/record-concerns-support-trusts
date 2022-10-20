using ConcernsCaseWork.Models.CaseActions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Pages.Validators
{
	public interface ICaseActionValidationStrategy
	{
		string Validate(IEnumerable<CaseActionModel> caseActions);
	}
}


