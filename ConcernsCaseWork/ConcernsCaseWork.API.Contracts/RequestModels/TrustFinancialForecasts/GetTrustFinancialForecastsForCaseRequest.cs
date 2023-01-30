namespace ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;

public record GetTrustFinancialForecastsForCaseRequest
{
	public int CaseUrn { get; init; }
	
	public virtual bool IsValid() => CaseUrn > 0;
}
