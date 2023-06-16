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

	public bool? Required { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> Validate(DisplayName ?? validationContext.DisplayName);

	private IEnumerable<ValidationResult> Validate(string displayName)
	{
		var result = new List<ValidationResult>();

		if (Required == true && string.IsNullOrEmpty(StringContents))
		{
			result.Add(new ValidationResult($"{displayName} is required", new[] { displayName }));
		}

		if (MaxLength > 0 && StringContents?.Length > MaxLength)
		{
			result.Add(new ValidationResult($"{displayName} must be {MaxLength} characters or less", new[] { displayName }));
		}

		return result;
	}
}