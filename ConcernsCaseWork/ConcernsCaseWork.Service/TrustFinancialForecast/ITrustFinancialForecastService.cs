using ConcernsCaseWork.API.Contracts.TrustFinancialForecast;

namespace ConcernsCaseWork.Service.TrustFinancialForecast;

public interface ITrustFinancialForecastService
{
	Task<string> Create(CreateTrustFinancialForecastRequest request);
	Task<TrustFinancialForecastResponse> GetById(GetTrustFinancialForecastByIdRequest request);
	Task Update(UpdateTrustFinancialForecastRequest request);
	Task<IEnumerable<TrustFinancialForecastResponse>> GetAllForCase(int caseUrn);
	Task Close(CloseTrustFinancialForecastRequest request);
}
