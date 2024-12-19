using ConcernsCaseWork.API.Contracts.TrustFinancialForecast;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.TrustFinancialForecast
{
	public class DeleteTrustFinancialForecast(ITrustFinancialForecastGateway financialForecastGateway) : IUseCaseAsync<DeleteTrustFinancialForecastRequest, int>
	{
		public Task<int> Execute(DeleteTrustFinancialForecastRequest request, CancellationToken cancellationToken)
		{
			financialForecastGateway.Delete(request.TrustFinancialForecastId);
			return Task.FromResult(request.TrustFinancialForecastId);
		}
	}
}
