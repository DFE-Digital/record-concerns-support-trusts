using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.Data.Exceptions;
using ConcernsCaseWork.Data.Gateways;
using NotFoundException = ConcernsCaseWork.API.Exceptions.NotFoundException;

namespace ConcernsCaseWork.API.UseCases.CaseActions.TrustFinancialForecast;

public class CloseTrustFinancialForecast : IUseCaseAsync<CloseTrustFinancialForecastRequest, int>
{
	private readonly IConcernsCaseGateway _concernsCaseGateway;
	private readonly ITrustFinancialForecastGateway _trustFinancialForecastGateway;

	public CloseTrustFinancialForecast(IConcernsCaseGateway concernsCaseGateway, ITrustFinancialForecastGateway trustFinancialForecastGateway)
	{
		_concernsCaseGateway = Guard.Against.Null(concernsCaseGateway);
		_trustFinancialForecastGateway = Guard.Against.Null(trustFinancialForecastGateway);
	}

	public async Task<int> Execute(CloseTrustFinancialForecastRequest request, CancellationToken cancellationToken)
	{
		EnsureRequestIsValid(request);
		
		await EnsureCaseExists(request.CaseUrn, cancellationToken);

		await EnsureTrustFinancialForecastCanBeEdited(request, cancellationToken);
			
		return await _trustFinancialForecastGateway.Close(request, cancellationToken);
	}

	private static void EnsureRequestIsValid(CloseTrustFinancialForecastRequest request)
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

	private async Task EnsureTrustFinancialForecastCanBeEdited(GetTrustFinancialForecastByIdRequest request, CancellationToken cancellationToken)
	{
		var trustFinancialForecast = await _trustFinancialForecastGateway
			.GetById(request, cancellationToken);

		if (trustFinancialForecast is null)
		{
			throw new NotFoundException($"Trust Financial Forecast with Id {request.TrustFinancialForecastId} not found");
		}

		if (trustFinancialForecast.ClosedAt.HasValue)
		{
			throw new StateChangeNotAllowedException($"Cannot close Trust Financial Forecast with Id {request.TrustFinancialForecastId} as it is already closed.");
		}
	}
}