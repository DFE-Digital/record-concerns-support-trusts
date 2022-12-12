using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;

namespace ConcernsCaseWork.Data.Gateways;

public abstract record CaseSummaryVm
{
	public long CaseUrn { get; set; }
	public string CreatedBy { get; set; }
	public DateTime CreatedAt { get; set; }
	public ConcernsRating Rating { get; set; }
	public string StatusName { get; set; }
	public string TrustUkPrn { get; set; }
	public DateTime UpdatedAt { get; set; }
	public IEnumerable<Decision> Decisions { get; set; }
	public IEnumerable<Action> FinancialPlanCases { get; set; }
	public IEnumerable<Action> NoticesToImprove { get; set; }
	public IEnumerable<Action> NtiWarningLetters { get; set; }
	public IEnumerable<Action> NtisUnderConsideration { get; set; }
	public IEnumerable<Action> SrmaCases { get; set; }
	
	public record Action(DateTime CreatedAt, DateTime? ClosedAt, string Name);
	public record Concern(string Name, ConcernsRating Rating, DateTime CreatedAt);
}

public record ActiveCaseSummaryVm : CaseSummaryVm
{
	public IEnumerable<Concern> ActiveConcerns { get; set; }
}

public record ClosedCaseSummaryVm : CaseSummaryVm
{
	public DateTime ClosedAt { get; set; }
	public IEnumerable<Concern> ClosedConcerns { get; set; }
}