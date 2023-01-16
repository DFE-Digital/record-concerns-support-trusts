using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.TrustFinancialForecast;

public class CloseTrustFinancialForecast : IUseCaseAsync<CloseTrustFinancialForecastRequest, int>
{
	private readonly IConcernsCaseGateway _concernsCaseGateway;
	private readonly ITrustFinancialForecastGateway _trustFinancialForecastGateway;

	public CloseTrustFinancialForecast(IConcernsCaseGateway concernsCaseGateway, ITrustFinancialForecastGateway trustFinancialForecastGateway)
	{
		_concernsCaseGateway = concernsCaseGateway ?? throw new ArgumentNullException(nameof(concernsCaseGateway));
		_trustFinancialForecastGateway = trustFinancialForecastGateway ?? throw new ArgumentNullException(nameof(trustFinancialForecastGateway));
	}

	public async Task<int> Execute(CloseTrustFinancialForecastRequest request, CancellationToken cancellationToken)
	{
		_ = request ?? throw new ArgumentNullException(nameof(request));

		if (!request.IsValid())
		{
			throw new ArgumentException("Request is not valid", nameof(request));
		}

		if (! await _concernsCaseGateway.CaseExists(request.CaseUrn, cancellationToken))
		{
			throw new InvalidOperationException($"The case for urn {request.CaseUrn}, was not found");
		}
            
		return await _trustFinancialForecastGateway.Close(request, cancellationToken);
	}
}