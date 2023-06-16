using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Pages.Validators
{
	public class NTIUnderConsiderationValidator : ICaseActionValidationStrategy
	{
		public string Validate(IEnumerable<CaseActionModel> caseActions)
		{
			var ntiUC = caseActions.Where(ca => ca is NtiUnderConsiderationModel).ToList();

			var errorMessage = ntiUC.Any(f => !f.ClosedAt.HasValue) ? "Close NTI: Under consideration" : string.Empty;

			return errorMessage;
		}
	}
}
