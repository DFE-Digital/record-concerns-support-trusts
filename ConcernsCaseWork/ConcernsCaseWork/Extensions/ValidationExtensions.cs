using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ConcernsCaseWork.Extensions
{
	public static class ValidationExtensions
	{
		public static List<ValidationResult> ToValidationResult(this ModelStateDictionary modelState)
		{
			var result = new List<ValidationResult>();

			foreach (var modelStateEntry in modelState)
			{
				result.AddRange(modelStateEntry.Value.Errors.Select(e => new ValidationResult(e.ErrorMessage)));
			}

			return result;
		}
		
		public static IEnumerable<KeyValuePair<string, string>> GetValidationMessages(this ModelStateDictionary modelStateDictionary)
		{
			return modelStateDictionary
				.Where(s => s.Value?.ValidationState == ModelValidationState.Invalid)
				.SelectMany(
					keyValue => keyValue.Value.Errors,
					(keyValue, modelError) => new KeyValuePair<string, string>(keyValue.Key, modelError?.ErrorMessage));
		}
	}
}
