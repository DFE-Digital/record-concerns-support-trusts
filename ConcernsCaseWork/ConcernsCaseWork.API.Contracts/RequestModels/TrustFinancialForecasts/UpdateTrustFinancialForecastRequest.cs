namespace ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;

public record UpdateTrustFinancialForecastRequest : CreateTrustFinancialForecastRequest
{
	public int TrustFinancialForecastId { get; set; }
	
	public override bool IsValid() => base.IsValid() 
	                                  && TrustFinancialForecastId > 0;
}
