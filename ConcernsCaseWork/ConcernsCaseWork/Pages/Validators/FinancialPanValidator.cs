using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Pages.Validators
{
	public class FinancialPanValidator : ICaseActionValidationStrategy
	{
		public string Validate(IEnumerable<CaseActionModel> caseActions)
		{
			var financialPlans = caseActions.Where(ca => ca is FinancialPlanModel).ToList();

			var errorMessage = financialPlans.Any(f => !f.ClosedAt.HasValue) ? "Resolve Financial Plan" : string.Empty;

			return errorMessage;
		}
	}
}

