using Microsoft.Graph.Models.TermStore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ConcernsCaseWork.Models;

public record RadioButtonsUiComponent(string ElementRootId, string Name, string Heading) : BaseUiComponent(ElementRootId, Name, Heading), IValidatableObject
{
	public IEnumerable<SimpleRadioItem> RadioItems { get; set; } = new List<SimpleRadioItem>();
	public int? SelectedId { get; set; }

	public IEnumerable<ValidationResult> Validate() => Validate(DisplayName);

	public string? HintFromPartialView { get; set; }

	public int? SelectedSubId { get; set; }

	public List<int>? OptionsWithSubItems { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	=> Validate(DisplayName ?? validationContext.DisplayName);

	public IEnumerable<ValidationResult> Validate(string displayName)
	{
		var result = new List<ValidationResult>();

		if (Required == true && SelectedId == null)
		{
			var requiredErrorText = !string.IsNullOrEmpty(ErrorTextForRequiredField) ? ErrorTextForRequiredField : $"Select {displayName}";

			result.Add(new ValidationResult(requiredErrorText, new[] { displayName }));
		}

		var hasSubItem = OptionsWithSubItems?.Any(o => o == SelectedId);

		if (hasSubItem == true && !SelectedSubId.HasValue)
		{
			result.Add(new ValidationResult($"Select sub {displayName}", new[] { displayName }));
		}

		return result;
	}
}