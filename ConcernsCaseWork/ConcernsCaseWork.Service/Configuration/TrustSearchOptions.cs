namespace ConcernsCaseWork.Service.Configuration
{
	public class TrustSearchOptions
	{
		public const string Cache = "TramsApi";

		public int MilliSecondPauseBeforeSeach { get; set; }
		public int MinCharsRequiredToSeach { get; set; }
		public int TrustsLimitByPage { get; set; }

		public int TrustsPerPage { get; set; }

		public string ShowWarningWhenTooManySearchResults { get; set; }
	}
}