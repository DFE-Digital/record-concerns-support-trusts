using System.Collections.Generic;

namespace ConcernsCaseWork.Models;


public abstract record PagedCaseSummaryModel
{
	public int Page { get; set; }
	public int RecordCount { get; set; }
	public string NextPageUrl { get; set; }
	public bool HasNext { get; set; }
	public bool HasPrevious { get; set; }
}

public record PagedActiveCases : PagedCaseSummaryModel
{
	public IEnumerable<ActiveCaseSummaryModel> ActiveCases { get; set; }
}

public record PagedClosedCases : PagedCaseSummaryModel
{
	public IEnumerable<ClosedCaseSummaryModel> ClosedCases { get; set; }
}


public abstract record CaseSummaryModel
{
	public long CaseUrn { get; set; }
	public string CreatedBy { get; set; }
	public string CreatedAt { get; set; }
	public bool IsMoreActionsAndDecisions { get; set; }
	public string StatusName { get; set; }
	public string TrustName { get; set; }
	public string UpdatedAt { get; set; }
}

public record ActiveCaseSummaryModel : CaseSummaryModel
{
	public string[] ActiveActionsAndDecisions { get; set; }
	public IEnumerable<string> ActiveConcerns { get; set; }
	public RatingModel Rating { get; set; }
	
	
	
	
}

public record ClosedCaseSummaryModel : CaseSummaryModel
{
	public string ClosedAt { get; set; }
	public string[] ClosedActionsAndDecisions { get; set; }
	public IEnumerable<string> ClosedConcerns { get; set; }
	
	
}