using ConcernsCaseWork.Service.Ratings;
using Newtonsoft.Json;
using System.Text.Json;

namespace ConcernsCaseWork.Service.Cases;



public abstract record CaseSummaryDto
{
	public long CaseUrn { get; set; }
	public string CreatedBy { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	public string StatusName { get; set; }
	public RatingDto Rating { get; set; }
	public string TrustUkPrn { get; set; }
	public IEnumerable<ActionDecisionSummaryDto> Decisions { get; set; }
	public IEnumerable<ActionDecisionSummaryDto> FinancialPlanCases { get; set; }
	public IEnumerable<ActionDecisionSummaryDto> NoticesToImprove { get; set; }
	public IEnumerable<ActionDecisionSummaryDto> NtiWarningLetters { get; set; }
	public IEnumerable<ActionDecisionSummaryDto> NtisUnderConsideration { get; set; }
	public IEnumerable<ActionDecisionSummaryDto> SrmaCases { get; set; }
	public IEnumerable<ActionDecisionSummaryDto> TrustFinancialForecasts { get; set; }
	
	public record ActionDecisionSummaryDto(DateTime CreatedAt, DateTime? ClosedAt, string Name);

	public record ConcernSummaryDto(string Name, RatingDto Rating, DateTime CreatedAt);
}

public record ActiveCaseSummaryDto : CaseSummaryDto
{
	public IEnumerable<ConcernSummaryDto> ActiveConcerns { get; set; }
	
	
}

public record ClosedCaseSummaryDto : CaseSummaryDto
{
	
	public DateTime ClosedAt { get; set; }
	public IEnumerable<ConcernSummaryDto> ClosedConcerns { get; set; }
}


public record PagedCasesDto
{
	
	public  Paging PageData { get; set; }

	public List<ClosedCaseSummaryDto> ClosedCases { get; set; }
	public List<ActiveCaseSummaryDto> ActiveCases { get; set; }

}
public record Paging
{
	public int Page { get; set; }
	public int RecordCount { get; set; }
	public string NextPageUrl { get; set; }
	public bool HasNext { get; set; }
	public bool HasPrevious { get; set; }
	public string Sort { get; set; }
	public string SearchPhrase { get; set; }
}
public record ActivePagedCasesDto 

{
	public long CaseUrn { get; set; }
	public string CreatedBy { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	public string StatusName { get; set; }
	public RatingDto Rating { get; set; }
	public string TrustUkPrn { get; set; }
	public IEnumerable<ActionDecisionSummaryDto> Decisions { get; set; }
	public IEnumerable<ActionDecisionSummaryDto> FinancialPlanCases { get; set; }
	public IEnumerable<ActionDecisionSummaryDto> NoticesToImprove { get; set; }
	public IEnumerable<ActionDecisionSummaryDto> NtiWarningLetters { get; set; }
	public IEnumerable<ActionDecisionSummaryDto> NtisUnderConsideration { get; set; }
	public IEnumerable<ActionDecisionSummaryDto> SrmaCases { get; set; }
	public IEnumerable<ActionDecisionSummaryDto> TrustFinancialForecasts { get; set; }
	public IEnumerable<ConcernSummaryDto> ActiveCases { get; set; }
	public record ActionDecisionSummaryDto(DateTime CreatedAt, DateTime? ClosedAt, string Name);

	//[JsonProperty("activeConcerns")]
	public record ConcernSummaryDto(string Name, RatingDto Rating, DateTime CreatedAt);
	
	
}

// public record ClosedPagedCasesDto : PagedCasesDto
// 	{	
// 		[JsonProperty("closedConcerns")]
// 	public List<ClosedCaseSummaryDto> Cases { get; set; }
// 	}
