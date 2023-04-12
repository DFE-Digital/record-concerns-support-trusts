using ConcernsCaseWork.Models.Validatable;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.Models;

public record OptionalDateTimeUiComponent(string ElementRootId, string Name, string Heading) : BaseUiComponent(ElementRootId, Name, Heading), IValidatableObject
{
	/// <summary>
	/// MVC always validates the children first
	/// If the child is invalid it never evaluates the parent
	/// We are already evaluating the child as part of our parent component
	/// The other solution is to remove the IValidatable from the child, but that is used on other screens
	/// </summary>
	[ValidateNever]
	public OptionalDateModel Date { get; set; }

	public IEnumerable<ValidationResult> Validate() => Validate(DisplayName);

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	=> Validate(DisplayName ?? validationContext.DisplayName);

	public IEnumerable<ValidationResult> Validate(string displayName)
	{
		var result = new List<ValidationResult>();

		if (Required == true && (Date == null || Date.IsEmpty()))
		{
			result.Add(new ValidationResult($"{displayName}: Please enter a date", new[] { displayName }));
		}

		if (Date != null)
		{
			result.AddRange(Date.Validate(displayName));
		}

		return result;
	}
}