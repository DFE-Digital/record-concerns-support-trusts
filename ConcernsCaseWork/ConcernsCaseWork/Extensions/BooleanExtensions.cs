namespace ConcernsCaseWork.Extensions
{
	public static class BooleanExtensions
	{
		public static int? ToInt(this bool? value)
		{
			return value.HasValue ? (value.Value ? 1 : 0) : null;
		}
	}
}
