using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.ResponseModels;

public record ActiveCaseSummaryResponse
{
	public long CaseUrn { get; set; }
	public string CreatedBy { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	public string StatusName { get; set; }
	public ConcernsRatingResponse Rating { get; set; }
	public string TrustUkPrn { get; set; }
	public IEnumerable<string> ActiveConcerns { get; set; }
	public IEnumerable<string> Decisions { get; set; }
	public IEnumerable<Summary> FinancialPlanCases { get; set; }
	public IEnumerable<Summary> NoticesToImprove { get; set; }
	public IEnumerable<Summary> NtiWarningLetters { get; set; }
	public IEnumerable<Summary> NtisUnderConsideration { get; set; }
	public IEnumerable<Summary> SrmaCases { get; set; }
	
}