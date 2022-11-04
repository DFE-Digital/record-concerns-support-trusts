using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;

namespace ConcernsCaseWork.Pages.Validators
{
	public interface ICaseActionValidationStrategy
	{
		string Validate(IEnumerable<CaseActionModel> caseActions);
	}
}
