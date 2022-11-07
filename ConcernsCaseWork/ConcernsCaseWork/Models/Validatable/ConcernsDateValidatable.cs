using ConcernsCaseWork.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ConcernsCaseWork.Models.Validatable;

public record ConcernsDateValidatable : IValidatableObject
{
	public string Day { get; set; }
	public string Month { get; set; }
	public string Year { get; set; }
		
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		var result = new List<ValidationResult>();
		
		if (!result.Any())
		{
			if (!DateTimeHelper.TryParseExact(this.ToString(), out _))
			{
				result.Add(new ValidationResult($"Enter a valid date."));
			}
		}

		return result;
	}

	public override string ToString() =>  $"{Day}-{Month}-{Year}";
}