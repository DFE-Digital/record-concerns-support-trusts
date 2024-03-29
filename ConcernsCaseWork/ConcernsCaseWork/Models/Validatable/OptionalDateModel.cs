﻿
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
		
		public OptionalDateModel(DateTime dateTime)
		{
			Day = dateTime.Day.ToString().PadLeft(2, '0');
			Month = dateTime.Month.ToString().PadLeft(2, '0');
			Year = dateTime.Year.ToString();
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
				result.Add(new ValidationResult($"{displayName} must be a complete date. For example, 17 5 2022", new []{ displayName } ));
				return result;
			}

			if (!DateTimeHelper.TryParseExact(ToString(), out _))
			{
				result.Add(new ValidationResult($"{displayName} must be a date that exists. For example, 17 5 2022", new []{ displayName }));
			}

			return result;
		}

		public bool IsEmpty()
		{
			return ToString() == "--";
		}

		public override string ToString() => $"{Day}-{Month}-{Year}";

		public DateTime? ToDateTime() 
		{
			if (this.IsEmpty())
			{
				return null;
			}

			return DateTimeHelper.ParseExact(this.ToString());
		}
	}
}
