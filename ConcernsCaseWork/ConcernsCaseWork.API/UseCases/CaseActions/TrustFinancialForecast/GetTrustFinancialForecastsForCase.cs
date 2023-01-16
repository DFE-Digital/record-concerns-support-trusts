using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.TrustFinancialForecast;

public class GetTrustFinancialForecastsForCase : IUseCaseAsync<GetTrustFinancialForecastForCaseRequest, IEnumerable<TrustFinancialForecastResponse>>
{
	private readonly IConcernsCaseGateway _concernsCaseGateway;
	private readonly ITrustFinancialForecastGateway _trustFinancialForecastGateway;

	public GetTrustFinancialForecastsForCase(IConcernsCaseGateway concernsCaseGateway, ITrustFinancialForecastGateway trustFinancialForecastGateway)
	{
		_concernsCaseGateway = concernsCaseGateway ?? throw new ArgumentNullException(nameof(concernsCaseGateway));
		_trustFinancialForecastGateway = trustFinancialForecastGateway ?? throw new ArgumentNullException(nameof(trustFinancialForecastGateway));
	}

	public async Task<IEnumerable<TrustFinancialForecastResponse>> Execute(GetTrustFinancialForecastForCaseRequest request, CancellationToken cancellationToken)
	{
		_ = request ?? throw new ArgumentNullException(nameof(request));

		if (! await _concernsCaseGateway.CaseExists(request.CaseUrn, cancellationToken))
		{
			throw new InvalidOperationException($"The case for urn {request.CaseUrn}, was not found");
		}
            
		return await _trustFinancialForecastGateway.GetAllForCase(request, cancellationToken);
	}
}