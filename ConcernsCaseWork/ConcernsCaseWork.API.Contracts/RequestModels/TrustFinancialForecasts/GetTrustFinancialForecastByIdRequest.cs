namespace ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;

public record GetTrustFinancialForecastByIdRequest : GetTrustFinancialForecastForCaseRequest
{
	public int TrustFinancialForecastId { get; init; }
	
	public override bool IsValid() => base.IsValid() 
	                                  && TrustFinancialForecastId > 0;
}