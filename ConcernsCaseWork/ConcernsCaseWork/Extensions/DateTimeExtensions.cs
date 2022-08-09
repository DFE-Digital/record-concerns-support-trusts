using System;

namespace ConcernsCaseWork.Extensions
{
	public static class DateTimeExtensions
	{
		public static string ParseDay(this DateTime? dt) => dt.HasValue ? $"{dt.Value.Day:00}" : string.Empty;
		public static string ParseMonth(this DateTime? dt) => dt.HasValue ? $"{dt.Value.Month:00}" : string.Empty;
		public static string ParseYear(this DateTime? dt) => dt.HasValue ? $"{dt.Value.Year:00}" : string.Empty;
	}
}