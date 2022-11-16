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
	public IEnumerable<ConcernSummaryDto> ActiveConcerns { get; set; }
	public IEnumerable<ActionDecisionSummaryDto> Decisions { get; set; }
	public IEnumerable<ActionDecisionSummaryDto> FinancialPlanCases { get; set; }
	public IEnumerable<ActionDecisionSummaryDto> NoticesToImprove { get; set; }
	public IEnumerable<ActionDecisionSummaryDto> NtiWarningLetters { get; set; }
	public IEnumerable<ActionDecisionSummaryDto> NtisUnderConsideration { get; set; }
	public IEnumerable<ActionDecisionSummaryDto> SrmaCases { get; set; }
	
	public record ActionDecisionSummaryDto(DateTime CreatedAt, DateTime? ClosedAt, string Name);

	public record ConcernSummaryDto(string Name, RatingDto Rating, DateTime CreatedAt);
}