using System;
using System.Globalization;

namespace ConcernsCaseWork.Extensions
{
	public static class StringExtension
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

		public static int ParseToInt(this string input)
		{
			bool result = int.TryParse(input, out int value);
			return result ? value : 0;
		}

		public static string ToUrl(this string value)
		{
			return Uri.TryCreate(value, UriKind.Absolute, out Uri _) ? value : $"http://{value}";
		}

		public static (string typeUrn, string type, string subType) SplitType(this string type, string subType)
		{
			// When the type is Force majeure, the type urn is included on the form type property
			var splitType = type.Split(":");
			var splitSubType = subType.Split(":");
			string subTypeName = string.Empty;
			string typeUrn = string.Empty;
			string typeName;

			if (splitType.Length > 1)
			{
				// Get type urn from type
				typeUrn = splitType[0];
				typeName = splitType[1];
				
				return (typeUrn, typeName, subTypeName);
			}

			typeName = splitType[0];
			
			if (splitSubType.Length > 1)
			{
				typeUrn = splitSubType[0];
				subTypeName = splitSubType[1];
			}
			else
			{
				subTypeName = splitSubType[0];
			}
			
			return (typeUrn, typeName, subTypeName);
		}
	}
}