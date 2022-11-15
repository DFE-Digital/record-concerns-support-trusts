using ConcernsCaseWork.Service.Ratings;

namespace ConcernsCaseWork.Service.Cases;

public record ActiveCaseSummaryDto
{
	public long CaseUrn { get; set; }
	public string CreatedBy { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	public string StatusName { get; set; }
	public RatingDto Rating { get; set; }
	public string TrustUkPrn { get; set; }
	public IEnumerable<string> ActiveConcerns { get; set; }
	public IEnumerable<Summary> FinancialPlanCases { get; set; }
	public IEnumerable<Summary> NoticesToImprove { get; set; }
	public IEnumerable<Summary> NtiWarningLetters { get; set; }
	public IEnumerable<Summary> NtisUnderConsideration { get; set; }
	public IEnumerable<Summary> SrmaCases { get; set; }
}
public record Summary(DateTime CreatedAt, DateTime? ClosedAt, string Name);