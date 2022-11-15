using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Models;

public record ActiveCaseSummaryModel
{
	public long CaseUrn { get; set; }
	public string CreatedBy { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	public string StatusName { get; set; }
	public RatingModel Rating { get; set; }
	public string TrustUkPrn { get; set; }
	public string[] ActiveActionsAndDecisions { get; set; }
	public IEnumerable<string> ActiveConcerns { get; set; }
}