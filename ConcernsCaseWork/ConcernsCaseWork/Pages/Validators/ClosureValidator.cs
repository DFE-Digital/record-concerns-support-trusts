using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using ConcernsCaseWork.Extensions;
using System;

namespace ConcernsCaseWork.Pages.Validators
{
	public static class ClosureValidator
	{
		public static bool IsValid(IFormCollection formCollection)
		{
			var caseOutcomes = formCollection["case-outcomes"];
			var monitoring = formCollection["monitoring"];
			var dayToReview = formCollection["dtr-day"];
			var monthToReview = formCollection["dtr-month"];
			var yearToReview = formCollection["dtr-year"];
			
			if (StringValues.IsNullOrEmpty(caseOutcomes) || StringValues.IsNullOrEmpty(monitoring)) return false;
			
			var isMonitoring = monitoring.ToString().ToBoolean();
			switch (isMonitoring)
			{
				case true when (StringValues.IsNullOrEmpty(dayToReview)
				                || StringValues.IsNullOrEmpty(monthToReview)
				                || StringValues.IsNullOrEmpty(yearToReview)):
					return false;
				case false:
					return true;
			}

			DateTime sourceDate = new DateTime(int.Parse(yearToReview), int.Parse(monthToReview), int.Parse(dayToReview), 0, 0, 0);
			DateTime utcTime = DateTime.SpecifyKind(sourceDate, DateTimeKind.Utc);
				
			var reviewDate = new DateTimeOffset(utcTime);
			var currentDate = DateTimeOffset.Now;
				
			return currentDate.CompareTo(reviewDate) < 0;
		}
	}
}