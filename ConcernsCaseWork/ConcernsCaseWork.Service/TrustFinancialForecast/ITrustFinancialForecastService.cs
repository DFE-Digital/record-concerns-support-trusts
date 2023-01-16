using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;

namespace ConcernsCaseWork.Service.TrustFinancialForecast;

public interface ITrustFinancialForecastService
{
	Task<string> Create(CreateTrustFinancialForecastRequest request);
	Task<TrustFinancialForecastResponse> GetById(GetTrustFinancialForecastByIdRequest request);
	Task Update(UpdateTrustFinancialForecastRequest request);
	Task<IEnumerable<TrustFinancialForecastResponse>> GetAllForCase(int caseUrn);
	Task Close(CloseTrustFinancialForecastRequest request);
}
