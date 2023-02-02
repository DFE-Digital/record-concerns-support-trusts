using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.Models;

public record TextAreaUiComponent(string ElementRootId, string Name, string Heading) : BaseUiComponent(ElementRootId, Name, Heading)
{
	public ValidateableString Text { get; set; }
}

public class ValidateableString : IValidatableObject
{
	public string StringContents { get; set; }
	public int MaxLength { get; set; }
	public string DisplayName { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> Validate(DisplayName ?? validationContext.DisplayName);

	private IEnumerable<ValidationResult> Validate(string displayName)
	{
		var result = new List<ValidationResult>();

		if (MaxLength > 0 && StringContents?.Length > MaxLength)
		{
			result.Add(new ValidationResult($"{displayName}: Exceeds maximum allowed length ({MaxLength} characters).", new[] { displayName }));
		}

		return result;
	}
}