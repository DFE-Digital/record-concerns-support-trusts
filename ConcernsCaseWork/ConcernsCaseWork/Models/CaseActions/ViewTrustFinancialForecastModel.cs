namespace ConcernsCaseWork.Models.CaseActions
{
	public class ViewTrustFinancialForecastModel
	{
		public string SRMAOfferedAfterTFF { get; set; }
		public string ForecastingToolRanAt { get; set; }
		public string WasTrustResponseSatisfactory { get; set; }
		public string Notes { get; set; }
		public string SFSOInitialReviewHappenedAt { get; set; }
		public string TrustRespondedAt { get; set; }
		public string DateOpened { get; set; }
		public string DateClosed { get; set; }
		public bool IsClosed { get; set; }
		public bool IsEditable { get; set; }
	}
}
