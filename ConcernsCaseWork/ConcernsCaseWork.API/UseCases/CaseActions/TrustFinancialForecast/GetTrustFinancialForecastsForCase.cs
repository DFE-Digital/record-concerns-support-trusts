using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.TrustFinancialForecast;

public class GetTrustFinancialForecastsForCase : IUseCaseAsync<GetTrustFinancialForecastsForCaseRequest, IEnumerable<TrustFinancialForecastResponse>>
{
	private readonly IConcernsCaseGateway _concernsCaseGateway;
	private readonly ITrustFinancialForecastGateway _trustFinancialForecastGateway;

	public GetTrustFinancialForecastsForCase(IConcernsCaseGateway concernsCaseGateway, ITrustFinancialForecastGateway trustFinancialForecastGateway)
	{
		_concernsCaseGateway = concernsCaseGateway ?? throw new ArgumentNullException(nameof(concernsCaseGateway));
		_trustFinancialForecastGateway = trustFinancialForecastGateway ?? throw new ArgumentNullException(nameof(trustFinancialForecastGateway));
	}

	public async Task<IEnumerable<TrustFinancialForecastResponse>> Execute(GetTrustFinancialForecastsForCaseRequest request, CancellationToken cancellationToken)
	{
		EnsureRequestIsValid(request);

		await EnsureCaseExists(request.CaseUrn, cancellationToken);
            
		return await _trustFinancialForecastGateway.GetAllForCase(request, cancellationToken);
	}
	
	private static void EnsureRequestIsValid(GetTrustFinancialForecastsForCaseRequest request)
	{
		if (request is null)
		{
			throw new ArgumentNullException(nameof(request));
		}
		
		if (!request.IsValid())
		{
			throw new ArgumentException("Request is not valid", nameof(request));
		}
	}

	private async Task EnsureCaseExists(int caseUrn, CancellationToken cancellationToken)
	{
		if (! await _concernsCaseGateway.CaseExists(caseUrn, cancellationToken))
		{
			throw new NotFoundException($"Concerns Case {caseUrn} not found");
		}
	}
}