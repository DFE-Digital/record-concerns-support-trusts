using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using System;
using ConcernsCaseWork.Extensions;

namespace ConcernsCaseWork.Pages.Base
{
	public class AbstractPageModel : PageModel
	{
		internal const string ErrorOnGetPage = "An error occurred loading the page, please try again. If the error persists contact the service administrator.";
		internal const string ErrorOnPostPage = "An error occurred posting the form, please try again. If the error persists contact the service administrator.";

		protected static bool IsValidConcernType(StringValues type, ref StringValues subType, StringValues ragRating, StringValues trustUkPrn)
		{
			const string forceMajeure = "Force Majeure";
			
			if (string.IsNullOrEmpty(type) 
			    || string.IsNullOrEmpty(ragRating)
			    || string.IsNullOrEmpty(trustUkPrn)) 
				return false;

			if (!type.ToString().Equals(forceMajeure, StringComparison.OrdinalIgnoreCase)
			    && string.IsNullOrEmpty(subType))
			{
				return false;
			}

			if (type.ToString().Equals(forceMajeure, StringComparison.OrdinalIgnoreCase)) {
				// UI component when type is forceMajeure brings data on subType
				// Forcing value to empty
				subType = StringValues.Empty;
			}

			return true;
		}

		protected static bool IsValidClosure(StringValues caseOutcomes, StringValues monitoring, 
			StringValues dayToReview, StringValues monthToReview, StringValues yearToReview)
		{
			if (string.IsNullOrEmpty(caseOutcomes) || string.IsNullOrEmpty(monitoring)) return false;
			
			var isMonitoring = monitoring.ToString().ToBoolean();
			if (isMonitoring && (string.IsNullOrEmpty(dayToReview)
			                     || string.IsNullOrEmpty(monthToReview)
			                     || string.IsNullOrEmpty(yearToReview)))
				return false;
			
			DateTime sourceDate = new DateTime(int.Parse(yearToReview), int.Parse(monthToReview), int.Parse(dayToReview), 0, 0, 0);
			DateTime utcTime = DateTime.SpecifyKind(sourceDate, DateTimeKind.Utc);
			
			var reviewDate = new DateTimeOffset(utcTime);
			var currentDate = DateTimeOffset.Now;
			
			return currentDate.CompareTo(reviewDate) >= 0;
		}
	}
}