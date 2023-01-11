using ConcernsCaseWork.API.Contracts.Constants;
using ConcernsCaseWork.API.Contracts.Enums.TrustFinancialForecast;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;

public record BaseTrustFinancialForecastRequest
{
	[Range(1, int.MaxValue, ErrorMessage = "The CaseUrn must be greater than zero")]
	public int CaseUrn { get; set; }

	public SRMAOfferedAfterTFF? SRMAOfferedAfterTFF { get; set; }
	public ForecastingToolRanAt? ForecastingToolRanAt { get; set; }
	public WasTrustResponseSatisfactory? WasTrustResponseSatisfactory { get; set; }

	[StringLength(TrustFinancialForecastConstants.MaxNotesLength, ErrorMessage  = "Notes must be 2000 characters or less", MinimumLength = 0)]
	public string Notes { get; set; }

	public DateTimeOffset? SFSOInitialReviewHappenedAt { get; set; }
	
	public DateTimeOffset? TrustRespondedAt { get; set; }

	public virtual bool IsValid()
	{
		return true;
	}
}