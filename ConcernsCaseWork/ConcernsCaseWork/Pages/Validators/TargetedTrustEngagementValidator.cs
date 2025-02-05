using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Pages.Validators
{
	public class TargetedTrustEngagementValidator : ICaseActionValidationStrategy
	{
		public string Validate(IEnumerable<CaseActionModel> caseActions)
		{
			var hasOpenActions = caseActions
				.Where(c => c is TargetedTrustEngagmentModel)
				.Any(c => !c.ClosedAt.HasValue);

			var result = hasOpenActions ? "Close TTE (targeted trust engagement)" : string.Empty;

			return result;
		}

		public string ValidateDelete(IEnumerable<CaseActionModel> caseActions)
		{
			var hasOpenActions = caseActions
				.Any(c => c is TargetedTrustEngagmentModel);

			var result = hasOpenActions ? "Delete TTE (targeted trust engagement)" : string.Empty;

			return result;
		}
	}
}
