namespace ConcernsCaseWork.API.Contracts.TrustFinancialForecast;

public record UpdateTrustFinancialForecastRequest : CreateTrustFinancialForecastRequest
{
	public int TrustFinancialForecastId { get; set; }

	public override bool IsValid() => base.IsValid()
									  && TrustFinancialForecastId > 0;
}
