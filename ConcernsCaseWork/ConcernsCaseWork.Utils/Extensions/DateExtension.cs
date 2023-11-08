﻿using System;

namespace ConcernsCaseWork.Utils.Extensions
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

		public static string ToDayMonthYear(this DateTime value)
		{
			return value.ToString("dd-MM-yyyy");
		}

		public static string ToDayMonthYear(this DateTime? value)
		{
			return value?.ToString("dd-MM-yyyy");
		}

		public static string ToDayMonthYearWithDefault(this DateTimeOffset value)
		{
			return value == default(DateTime) ? string.Empty : value.ToDayMonthYear();
		}
	}
}