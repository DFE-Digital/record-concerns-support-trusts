using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Extensions
{
	public static class ValidationExtensions
	{
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
