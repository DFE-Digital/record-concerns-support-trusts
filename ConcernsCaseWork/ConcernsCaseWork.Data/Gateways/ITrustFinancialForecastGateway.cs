using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.Data.Gateways;

public interface ITrustFinancialForecastGateway
{
	Task<int> Create(CreateTrustFinancialForecastRequest request, CancellationToken cancellationToken = default);
	Task<TrustFinancialForecast> GetById(int trustFinancialForecastId, CancellationToken cancellationToken = default);
	Task<ICollection<TrustFinancialForecast>> GetAllByCaseUrn(int caseUrn, CancellationToken cancellationToken = default);
	Task Update(UpdateTrustFinancialForecastRequest updatedTrustFinancialForecastRequest, CancellationToken cancellationToken = default);
}