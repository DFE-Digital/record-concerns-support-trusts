using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ConcernsCaseWork.Extensions
{
	public static class DisplayExtensions
	{
		public static string GetErrorStyleClass(this ModelStateDictionary modelState)
		{
			return !modelState.IsValid ? "govuk-form-group--error" : "";
		}

		public static string GetTextAreaErrorStyles(this ModelStateDictionary modelState, string field)
		{
			return modelState.IsValid is false && modelState.ContainsKey(field) ? "govuk-textarea--error" : string.Empty;
		}
	}
}
