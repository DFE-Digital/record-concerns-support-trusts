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

			var errorMessage = ntiModel.Any(f => !f.ClosedAt.HasValue) ? "Cancel, lift or close NTI: Notice to improve" : string.Empty;

			return errorMessage;
		}
	}
}
