using System;
using System.Collections.Generic;
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

		public static (string typeId, string type, string subType) SplitType(this string type, string subType)
		{
			// When the type is Force majeure, the type urn is included on the form type property
			var splitType = type.Split(":");
			var splitSubType = subType.Split(":");
			string subTypeName = string.Empty;
			string typeId = string.Empty;
			string typeName;

			if (splitType.Length > 1)
			{
				// Get type urn from type
				typeId = splitType[0];
				typeName = splitType[1];
				
				return (typeId, typeName, subTypeName);
			}

			typeName = splitType[0];
			
			if (splitSubType.Length > 1)
			{
				typeId = splitSubType[0];
				subTypeName = splitSubType[1];
			}
			else
			{
				subTypeName = splitSubType[0];
			}
			
			return (typeId, typeName, subTypeName);
		}
		
		public static T ToEnum<T>(this string value)
		{
			return (T) Enum.Parse(typeof(T), value, true);
		}
		
		public static string GetValueOrNullIfWhitespace(this string value) => string.IsNullOrWhiteSpace(value) ? null : value;

		public static string FromEmailToFullName(this string email)
		{
			var fullName = email.Split('@')[0];
			var names = fullName.Split('.');

			var parsedNames = new List<string>();

			foreach (var name in names)
			{
				var parsedName = name.ToLower().CapitaliseFirstChar();
				parsedNames.Add(parsedName);
			}

			var result = string.Join(' ', parsedNames);

			return result;
		}

		private static string CapitaliseFirstChar(this string input)
		{
			var charArray = input.ToCharArray();

			var capitalChar = charArray[0].ToString().ToUpper();
			charArray[0] = char.Parse(capitalChar);


			var result = new string(charArray);

			return result;
		}

	}
}