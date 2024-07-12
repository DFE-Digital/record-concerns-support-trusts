using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Pages.Validators
{
	public class DecisionValidator : ICaseActionValidationStrategy
	{
		public string Validate(IEnumerable<CaseActionModel> caseActions)
		{
			var hasOpenActions = caseActions
				.Where(c => c is DecisionSummaryModel)
				.Any(c => !c.ClosedAt.HasValue);

			var result = hasOpenActions ? "Close decisions" : string.Empty;

			return result;
		}

		public string ValidateDelete(IEnumerable<CaseActionModel> caseActions)
		{
			var hasOpenActions = caseActions.Any(c => c is DecisionSummaryModel);

			var result = hasOpenActions ? "Delete decisions" : string.Empty;

			return result;
		}
	}
}
