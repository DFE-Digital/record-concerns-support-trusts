using ConcernsCaseWork.API.Contracts.Constants;
using ConcernsCaseWork.API.Contracts.Enums.TrustFinancialForecast;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;

public record CreateTrustFinancialForecastRequest
{
	public int CaseUrn { get; set; }
	public SRMAOfferedAfterTFF? SRMAOfferedAfterTFF { get; set; }
	public ForecastingToolRanAt? ForecastingToolRanAt { get; set; }
	public WasTrustResponseSatisfactory? WasTrustResponseSatisfactory { get; set; }
	public string Notes { get; set; }
	public DateTime? SFSOInitialReviewHappenedAt { get; set; }
	public DateTime? TrustRespondedAt { get; set; }

	public virtual bool IsValid() => CaseUrn > 0 && (Notes?.Length ?? 0) <= TrustFinancialForecastConstants.MaxNotesLength;
}