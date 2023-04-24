using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.Models;

public record RadioButtonsUiComponent(string ElementRootId, string Name, string Heading) : BaseUiComponent(ElementRootId, Name, Heading), IValidatableObject
{
	public IEnumerable<SimpleRadioItem> RadioItems { get; set; } = new List<SimpleRadioItem>();
	public int? SelectedId { get; set; }

	public IEnumerable<ValidationResult> Validate() => Validate(DisplayName);

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	=> Validate(DisplayName ?? validationContext.DisplayName);

	public IEnumerable<ValidationResult> Validate(string displayName)
	{
		var result = new List<ValidationResult>();

		if (Required == true && SelectedId == null)
		{
			var requiredErrorText = !string.IsNullOrEmpty(ErrorTextForRequiredField) ? ErrorTextForRequiredField : $"{displayName}: Please enter a value";

			result.Add(new ValidationResult(requiredErrorText, new[] { displayName }));
		}

		return result;
	}
}