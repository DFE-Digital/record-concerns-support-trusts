using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Pages.Validators
{
	public class CaseActionValidator : ICaseActionValidator
	{
		private readonly IEnumerable<ICaseActionValidationStrategy> _strategies;

		public CaseActionValidator(IEnumerable<ICaseActionValidationStrategy> strategies)
		{
			_strategies = strategies;
		}

		public List<string> Validate(IEnumerable<CaseActionModel> caseActions)
		{
			List<string> validationMessages = new List<string>();

			foreach (var strategy in _strategies.ToList())
			{
				var validationErrorMessage = strategy.Validate(caseActions);

				if (!string.IsNullOrEmpty(validationErrorMessage))
				{
					validationMessages.Add(validationErrorMessage);
				}
			}

			return validationMessages;
		}
	}
}

