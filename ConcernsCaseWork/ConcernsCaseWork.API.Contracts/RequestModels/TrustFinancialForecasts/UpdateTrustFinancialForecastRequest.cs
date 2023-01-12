namespace ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;

public record UpdateTrustFinancialForecastRequest : BaseTrustFinancialForecastRequest
{
	public int TrustFinancialForecastId { get; set; }
}