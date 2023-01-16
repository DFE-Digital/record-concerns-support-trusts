using ConcernsCaseWork.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.Models.Validatable
{
	public record OptionalDateModel : IValidatableObject
	{
		public string DisplayName { get; set; }
		public string Day { get; set; }
		public string Month { get; set; }
		public string Year { get; set; }

		public OptionalDateModel() { }
		
		public OptionalDateModel(DateTimeOffset dateTimeOffset)
		{
			Day = dateTimeOffset.Day.ToString().PadLeft(2, '0');
			Month = dateTimeOffset.Month.ToString().PadLeft(2, '0');
			Year = dateTimeOffset.Year.ToString();
		}
		
		public IEnumerable<ValidationResult> Validate() => Validate(DisplayName);

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
			=> Validate(DisplayName ?? validationContext.DisplayName);

		public IEnumerable<ValidationResult> Validate(string displayName)
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
				result.Add(new ValidationResult($"{displayName}: Please enter a complete date DD MM YYYY",new []{ displayName } ));
				return result;
			}

			if (!DateTimeHelper.TryParseExact(ToString(), out _))
			{
				result.Add(new ValidationResult($"{displayName}: {ToString()} is an invalid date",new []{ displayName }));
			}

			return result;
		}

		public bool IsEmpty()
		{
			return ToString() == "--";
		}

		public override string ToString() => $"{Day}-{Month}-{Year}";

		public DateTimeOffset ToDateTimeOffset() => DateTimeHelper.ParseExact(this.ToString());
	}
}
