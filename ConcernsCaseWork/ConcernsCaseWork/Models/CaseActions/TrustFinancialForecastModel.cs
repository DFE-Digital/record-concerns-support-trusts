using ConcernsCaseWork.API.Contracts.Enums.TrustFinancialForecast;
using System;

namespace ConcernsCaseWork.Models.CaseActions;

public record TrustFinancialForecastModel
{
	public int TrustFinancialForecastId { get; set; }
	public int CaseUrn { get; set; }

	public SRMAOfferedAfterTFF? SRMAOfferedAfterTFF { get; set; }
	public ForecastingToolRanAt? ForecastingToolRanAt { get; set; }
	public WasTrustResponseSatisfactory? WasTrustResponseSatisfactory { get; set; }

	public string Notes { get; set; }

	public DateTimeOffset? SFSOInitialReviewHappenedAt { get; set; }
	
	public DateTimeOffset? TrustRespondedAt { get; set; }
}