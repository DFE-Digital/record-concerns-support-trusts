﻿using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Pages.Validators
{
	public class SRMAValidator : ICaseActionValidationStrategy
	{
		public string Validate(IEnumerable<CaseActionModel> caseActions)
		{
			var srma = caseActions.Where(ca => ca is SRMAModel).ToList();

			var errorMessage = srma.Any(f => !f.ClosedAt.HasValue) ? "Close SRMA action" : string.Empty;

			return errorMessage;
		}

		public string ValidateDelete(IEnumerable<CaseActionModel> caseActions)
		{
			var srma = caseActions.Any(ca => ca is SRMAModel);

			var errorMessage = srma ? "Delete SRMA action" : string.Empty;

			return errorMessage;
		}
	}
}
