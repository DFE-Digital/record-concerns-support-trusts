using ConcernsCaseWork.Models.CaseActions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Pages.Validators
{
	public class NTIUnderConsiderationValidator : ICaseActionValidationStrategy
	{
		public string Validate(IEnumerable<CaseActionModel> caseActions)
		{
			var ntiUC = caseActions.Where(ca => ca is NtiUnderConsiderationModel).ToList();

			var errorMessage = ntiUC.Any(f => !f.ClosedAt.HasValue) ? "Resolve NTI Under Consideration" : string.Empty;

			return errorMessage;
		}
	}
}

