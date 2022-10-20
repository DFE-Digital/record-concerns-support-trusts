using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;

namespace ConcernsCaseWork.Pages.Validators
{
	public interface ICaseActionValidator
	{
		List<string> Validate(IEnumerable<CaseActionModel> caseActions);
	}
}