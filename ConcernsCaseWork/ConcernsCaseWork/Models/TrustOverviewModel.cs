namespace ConcernsCaseWork.Models
{
	public class TrustOverviewModel
	{
		public TrustDetailsModel TrustDetailsModel { get; set; }

		public CaseSummaryGroupModel<ActiveCaseSummaryModel> ActiveCaseSummaryGroupModel { get; set; }

		public CaseSummaryGroupModel<ClosedCaseSummaryModel> ClosedCaseSummaryGroupModel { get; set; }
	}
}
