namespace ConcernsCaseWork.Utils.Extensions
{
	public static class IntegerExtensions
	{
		public static bool? ToBool(this int? value)
		{
			return value.HasValue ? value.Value == 1 ? true : false : null;
		}
	}
}
