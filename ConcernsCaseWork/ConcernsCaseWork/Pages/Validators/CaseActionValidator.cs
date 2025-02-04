using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Pages.Validators
{
	public class CaseActionValidator(IEnumerable<ICaseActionValidationStrategy> strategies) : ICaseActionValidator
	{
		public List<string> Validate(IEnumerable<CaseActionModel> caseActions)
		{
			List<string> validationMessages = [];

			foreach (var strategy in strategies.ToList())
			{
				var validationErrorMessage = strategy.Validate(caseActions);

				if (!string.IsNullOrEmpty(validationErrorMessage))
				{
					validationMessages.Add(validationErrorMessage);
				}
			}

			return validationMessages;
		}

		public List<string> ValidateDelete(IEnumerable<CaseActionModel> caseActions)
		{
			List<string> validationMessages = [];

			foreach (var strategy in strategies.ToList())
			{
				var validationErrorMessage = strategy.ValidateDelete(caseActions);

				if (!string.IsNullOrEmpty(validationErrorMessage))
				{
					validationMessages.Add(validationErrorMessage);
				}
			}

			return validationMessages;
		}
	}
}
