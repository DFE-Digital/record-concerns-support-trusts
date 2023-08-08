﻿using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.TrustFinancialForecast
{
	public class DeleteTrustFinancialForecast : IUseCaseAsync<DeleteTrustFinancialForecastRequest, int>
	{
		private readonly ITrustFinancialForecastGateway _trustFinancialForecastGateway;

		public DeleteTrustFinancialForecast(ITrustFinancialForecastGateway financialForecastGateway)
		{
			_trustFinancialForecastGateway = financialForecastGateway;
		}

		public Task<int> Execute(DeleteTrustFinancialForecastRequest request, CancellationToken cancellationToken)
		{
			_trustFinancialForecastGateway.Delete(request.TrustFinancialForecastId);
			return Task.FromResult(request.TrustFinancialForecastId);
		}
	}
}
