using System.ComponentModel;
using System.Globalization;

namespace ConcernsCaseWork.API.Extensions
{
	public static class EnumExtensions
	{
		public static string GetDescription<T>(this T e) where T : IConvertible
		{
			if (e is Enum)
			{
				Type type = e.GetType();

				foreach (int val in System.Enum.GetValues(type))
				{
					if (val == e.ToInt32(CultureInfo.InvariantCulture))
					{
						var memInfo = type.GetMember(type.GetEnumName(val));
						var descriptionAttribute = memInfo[0]
							.GetCustomAttributes(typeof(DescriptionAttribute), false)
							.FirstOrDefault() as DescriptionAttribute;

						if (descriptionAttribute != null)
						{
							return descriptionAttribute.Description;
						}
					}
				}
			}

			return null;
		}
	}
}
