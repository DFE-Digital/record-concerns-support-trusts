using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.Data.Gateways;

public interface ITrustFinancialForecastGateway
{
	Task<int> Create(CreateTrustFinancialForecastRequest request, CancellationToken cancellationToken = default);
	Task<TrustFinancialForecastResponse> GetById(GetTrustFinancialForecastByIdRequest request, CancellationToken cancellationToken = default);
	Task<ICollection<TrustFinancialForecastResponse>> GetAllForCase(GetTrustFinancialForecastsForCaseRequest request, CancellationToken cancellationToken = default);
	Task<int> Update(UpdateTrustFinancialForecastRequest updatedTrustFinancialForecastRequest, CancellationToken cancellationToken = default);
	Task<int> Close(CloseTrustFinancialForecastRequest request, CancellationToken cancellationToken = default);
}