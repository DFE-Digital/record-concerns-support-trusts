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
			result.Add(new ValidationResult($"Select {displayName}", new[] { displayName }));
		}

		return result;
	}
}