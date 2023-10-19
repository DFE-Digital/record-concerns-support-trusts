namespace ConcernsCaseWork.API.Contracts.TrustFinancialForecast;

public record GetTrustFinancialForecastByIdRequest : GetTrustFinancialForecastsForCaseRequest
{
	public int TrustFinancialForecastId { get; init; }

	public override bool IsValid() => base.IsValid()
									  && TrustFinancialForecastId > 0;
}
