using ConcernsCaseWork.API.Contracts.Constants;

namespace ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;

public record CloseTrustFinancialForecastRequest : GetTrustFinancialForecastByIdRequest
{
	public string Notes { get; set; }
	
	public override bool IsValid() => base.IsValid()
	                                  && (Notes?.Length ?? 0) <= TrustFinancialForecastConstants.MaxNotesLength;
}
