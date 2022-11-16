using System.Collections.Generic;

namespace ConcernsCaseWork.Models;

public record ActiveCaseSummaryModel
{
	public string[] ActiveActionsAndDecisions { get; set; }
	public IEnumerable<string> ActiveConcerns { get; set; }
	public long CaseUrn { get; set; }
	public string CreatedBy { get; set; }
	public string CreatedAt { get; set; }
	public bool IsMoreActionsAndDecisions { get; set; }
	public RatingModel Rating { get; set; }
	public string StatusName { get; set; }
	public string TrustName { get; set; }
	public string UpdatedAt { get; set; }
}