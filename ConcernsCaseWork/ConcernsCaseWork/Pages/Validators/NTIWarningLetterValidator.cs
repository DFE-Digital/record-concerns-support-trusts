﻿using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Pages.Validators
{
	public class NTIWarningLetterValidator : ICaseActionValidationStrategy
	{
		public string Validate(IEnumerable<CaseActionModel> caseActions)
		{
			var ntiWarningLetters = caseActions.Where(ca => ca is NtiWarningLetterModel).ToList();

			var errorMessage = ntiWarningLetters.Any(f => !f.ClosedAt.HasValue) ? "Close NTI: Warning letter" : string.Empty;

			return errorMessage;
		}

		public string ValidateDelete(IEnumerable<CaseActionModel> caseActions)
		{
			var ntiWarningLetters = caseActions.Any(ca => ca is NtiWarningLetterModel);

			var errorMessage = ntiWarningLetters ? "Delete NTI: Warning letter" : string.Empty;

			return errorMessage;
		}
	}
}
