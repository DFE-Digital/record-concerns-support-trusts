using System.Globalization;

namespace ConcernsCaseWork.Extensions
{
	public static class StringExtensions
	{
		public static bool ToBoolean(this string value)
		{
			return value.ToLower().Trim() switch
			{
				"true" => true,
				"t" => true,
				"yes" => true,
				"y" => true,
				"1" => true,
				"false" => false,
				"f" => false,
				"no" => false,
				"n" => false,
				"0" => false,
				_ => false
			};
		}

		public static string ToTitle(this string value)
		{
			return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
		}
	}
}