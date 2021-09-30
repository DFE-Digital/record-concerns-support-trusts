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
			
			if (StringValues.IsNullOrEmpty(type) 
			    || StringValues.IsNullOrEmpty(ragRating)
			    || StringValues.IsNullOrEmpty(trustUkPrn)) 
				return false;

			if (!type.ToString().Equals(forceMajeure, StringComparison.OrdinalIgnoreCase)
			    && StringValues.IsNullOrEmpty(subType))
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

		protected static bool IsValidEditConcernType(StringValues type, ref StringValues subType)
		{
			const string forceMajeure = "Force Majeure";
			
			if (StringValues.IsNullOrEmpty(type)) return false;

			if (!type.ToString().Equals(forceMajeure, StringComparison.OrdinalIgnoreCase)
			    && StringValues.IsNullOrEmpty(subType))
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
			if (StringValues.IsNullOrEmpty(caseOutcomes) || StringValues.IsNullOrEmpty(monitoring)) return false;
			
			var isMonitoring = monitoring.ToString().ToBoolean();
			if (isMonitoring && (StringValues.IsNullOrEmpty(dayToReview)
			                     || StringValues.IsNullOrEmpty(monthToReview)
			                     || StringValues.IsNullOrEmpty(yearToReview)))
				return false;
			
			DateTime sourceDate = new DateTime(int.Parse(yearToReview), int.Parse(monthToReview), int.Parse(dayToReview), 0, 0, 0);
			DateTime utcTime = DateTime.SpecifyKind(sourceDate, DateTimeKind.Utc);
			
			var reviewDate = new DateTimeOffset(utcTime);
			var currentDate = DateTimeOffset.Now;
			
			return currentDate.CompareTo(reviewDate) >= 0;
		}
	}
}