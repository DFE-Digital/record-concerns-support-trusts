namespace ConcernsCaseWork.Data.Tests.Helpers
{
	public static class TestExtensions
	{
		public static string ToKpiDateFormat(this DateTime date)
		{
			return date.ToString("dd-MM-yyyy");
		}
	}
}
