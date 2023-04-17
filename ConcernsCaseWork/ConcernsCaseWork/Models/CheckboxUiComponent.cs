using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.Models
{
	public record CheckboxUiComponent(string ElementRootId, string Name, string Heading) : BaseUiComponent(ElementRootId, Name, Heading), IValidatableObject
	{
		public bool Checked { get; set; }

		public string Text { get; set; }

		public string ErrorTextForRequiredField { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var result = new List<ValidationResult>();

			if (Required == true && !Checked)
			{
				var requiredErrorText = !string.IsNullOrEmpty(ErrorTextForRequiredField) ? ErrorTextForRequiredField : $"{DisplayName}: Please select";

				result.Add(new ValidationResult(requiredErrorText, new[] { DisplayName }));
			}

			return result;
		}
	}
}
