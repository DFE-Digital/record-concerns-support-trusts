namespace ConcernsCaseWork.API.Contracts.TrustFinancialForecast;

public record GetTrustFinancialForecastsForCaseRequest
{
	public int CaseUrn { get; init; }

	public virtual bool IsValid() => CaseUrn > 0;
}
