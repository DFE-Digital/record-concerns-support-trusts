namespace ConcernsCaseWork.API.Contracts.TrustFinancialForecast;

public record CloseTrustFinancialForecastRequest : GetTrustFinancialForecastByIdRequest
{
	public string Notes { get; set; }

	public override bool IsValid() => base.IsValid()
									  && (Notes?.Length ?? 0) <= TrustFinancialForecastConstants.MaxNotesLength;
}
