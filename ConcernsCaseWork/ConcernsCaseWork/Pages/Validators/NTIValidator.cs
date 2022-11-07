using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Pages.Validators
{
	public class NTIValidator : ICaseActionValidationStrategy
	{
		public string Validate(IEnumerable<CaseActionModel> caseActions)
		{
			var ntiModel = caseActions.Where(ca => ca is NtiModel).ToList();

			var errorMessage = ntiModel.Any(f => !f.ClosedAt.HasValue) ? "Resolve Notice To Improve" : string.Empty;

			return errorMessage;
		}
	}
}
