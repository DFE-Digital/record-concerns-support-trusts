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

			var result = hasOpenActions ? "Resolve Decision(s)" : string.Empty;

			return result;
		}
	}
}
