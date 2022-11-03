using System;
using System.Globalization;

namespace ConcernsCaseWork.Helpers
{

	public class DateTimeHelper
	{
		private static readonly string[] allowedFormats = new string[] { "dd-MM-yyyy", "dd/MM/yyyy", "d-M-yyyy", "d/M/yyyy" };
		private static readonly DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.InvariantCulture.DateTimeFormat;
		private static readonly DateTimeStyles dateTimeStyle = DateTimeStyles.None;

		public static bool TryParseExact(string value, out DateTime parsedDate)
		{
			return DateTime.TryParseExact(value, allowedFormats, dateTimeFormatInfo, dateTimeStyle, out parsedDate);
		}

		public static DateTime ParseExact(string value) 
		{
			return DateTime.ParseExact(value, allowedFormats, dateTimeFormatInfo, dateTimeStyle);
		}

	}
}
