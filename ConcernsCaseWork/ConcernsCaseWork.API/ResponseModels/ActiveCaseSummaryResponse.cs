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
	public IEnumerable<Concern> ActiveConcerns { get; set; }
	public IEnumerable<ActionOrDecision> Decisions { get; set; }
	public IEnumerable<ActionOrDecision> FinancialPlanCases { get; set; }
	public IEnumerable<ActionOrDecision> NoticesToImprove { get; set; }
	public IEnumerable<ActionOrDecision> NtiWarningLetters { get; set; }
	public IEnumerable<ActionOrDecision> NtisUnderConsideration { get; set; }
	public IEnumerable<ActionOrDecision> SrmaCases { get; set; }
	
	public record ActionOrDecision(DateTime CreatedAt, DateTime? ClosedAt, string Name);
	public record Concern(string Name, ConcernsRatingResponse Rating, DateTime CreatedAt);
}