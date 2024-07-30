using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Pages.Validators
{
	public class TrustFinancialForecastValidator : ICaseActionValidationStrategy
	{
		public string Validate(IEnumerable<CaseActionModel> caseActions)
		{
			var hasOpenActions = caseActions
				.Where(c => c is TrustFinancialForecastSummaryModel)
				.Any(c => !c.ClosedAt.HasValue);

			var result = hasOpenActions ? "Close trust financial forecast" : string.Empty;

			return result;
		}

		public string ValidateDelete(IEnumerable<CaseActionModel> caseActions)
		{
			var hasOpenActions = caseActions
				.Any(c => c is TrustFinancialForecastSummaryModel);

			var result = hasOpenActions ? "Delete trust financial forecast" : string.Empty;

			return result;
		}
	}
}
