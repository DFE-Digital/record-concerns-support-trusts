using System;

namespace ConcernsCaseWork.Extensions
{
	public static class DateExtension
	{
		public static string ToUserFriendlyDate(this DateTimeOffset value)
		{
			return value.ToString("d MMMM yyyy");
		}

		public static string ToDayMonthYear(this DateTimeOffset value)
		{
			return value.ToString("dd-MM-yyyy");
		}
	}
}