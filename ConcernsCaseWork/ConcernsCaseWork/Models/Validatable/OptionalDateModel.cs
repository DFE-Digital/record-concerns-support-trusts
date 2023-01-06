using ConcernsCaseWork.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.Models.Validatable
{
	public record OptionalDateModel : IValidatableObject
	{
		public string Day { get; set; }
		public string Month { get; set; }
		public string Year { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var result = new List<ValidationResult>();

			var dateValues = new List<string>() { Day, Month, Year };
			dateValues.RemoveAll(d => d == null);

			if (dateValues.Count == 0)
			{
				return result;
			}

			if (dateValues.Count != 3)
			{
				result.Add(new ValidationResult($"{validationContext.DisplayName}: Please enter a complete date DD MM YYYY"));
				return result;
			}

			if (!DateTimeHelper.TryParseExact(ToString(), out _))
			{
				result.Add(new ValidationResult($"{validationContext.DisplayName}: {ToString()} is an invalid date"));
			}

			return result;
		}

		public bool IsEmpty()
		{
			return ToString() == "--";
		}

		public override string ToString() => $"{Day}-{Month}-{Year}";
	}
}
