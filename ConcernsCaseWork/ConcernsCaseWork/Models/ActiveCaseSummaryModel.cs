using ConcernsCaseWork.API.Contracts.Case;
using System.Collections.Generic;
namespace ConcernsCaseWork.Models;

public abstract record CaseSummaryModel
{
	public long CaseUrn { get; set; }
	public string CreatedBy { get; set; }
	public string CreatedAt { get; set; }
	public bool IsMoreActionsAndDecisions { get; set; }
	public string StatusName { get; set; }
	public string TrustName { get; set; }
	public string UpdatedAt { get; set; }

	public Division? Division { get; set; }
	public string? Area { get; set; }
	public string TeamLedBy { get; set; }
}

public record ActiveCaseSummaryModel : CaseSummaryModel
{
	public string[] ActiveActionsAndDecisions { get; set; }
	public IEnumerable<string> ActiveConcerns { get; set; }
	public long RatingId { get; set; }
	public string CaseLastUpdatedAt { get; internal set; }
}

public record ClosedCaseSummaryModel : CaseSummaryModel
{
	public string ClosedAt { get; set; }
	public string[] ClosedActionsAndDecisions { get; set; }
	public IEnumerable<string> ClosedConcerns { get; set; }
}

public record CaseSummaryGroupModel<T> where T : CaseSummaryModel
{
	public CaseSummaryGroupModel()
	{
		Cases = new List<T>();
	}

	public List<T> Cases { get; set; }

	public PaginationModel Pagination { get; set; }
}