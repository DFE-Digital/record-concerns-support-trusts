using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.Data.Gateways;

public interface ITrustFinancialForecastGateway
{
	Task<TrustFinancialForecast> GetById(int trustFinancialForecastId, CancellationToken cancellationToken = default);
	Task<ICollection<TrustFinancialForecast>> GetAllForCase(int caseUrn, CancellationToken cancellationToken = default);
	Task<int> Update(TrustFinancialForecast trustFinancialForecast, CancellationToken cancellationToken = default);
}