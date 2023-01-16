using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.Data.Exceptions;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.TrustFinancialForecast;

public class UpdateTrustFinancialForecast : IUseCaseAsync<UpdateTrustFinancialForecastRequest, int>
{
	private readonly IConcernsCaseGateway _concernsCaseGateway;
	private readonly ITrustFinancialForecastGateway _trustFinancialForecastGateway;

	public UpdateTrustFinancialForecast(IConcernsCaseGateway concernsCaseGateway, ITrustFinancialForecastGateway trustFinancialForecastGateway)
	{
		_concernsCaseGateway = concernsCaseGateway ?? throw new ArgumentNullException(nameof(concernsCaseGateway));
		_trustFinancialForecastGateway = trustFinancialForecastGateway ?? throw new ArgumentNullException(nameof(trustFinancialForecastGateway));
	}

	public async Task<int> Execute(UpdateTrustFinancialForecastRequest request, CancellationToken cancellationToken)
	{
		EnsureRequestIsValid(request);

		await EnsureCaseExists(request.CaseUrn, cancellationToken);

		await EnsureTrustFinancialForecastCanBeEdited(request, cancellationToken);
            
		return await _trustFinancialForecastGateway.Update(request, cancellationToken);
	}
	
	private static void EnsureRequestIsValid(UpdateTrustFinancialForecastRequest request)
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

	private async Task EnsureTrustFinancialForecastCanBeEdited(UpdateTrustFinancialForecastRequest request, CancellationToken cancellationToken)
	{
		var getRequest = new GetTrustFinancialForecastByIdRequest { TrustFinancialForecastId = request.TrustFinancialForecastId, CaseUrn = request.CaseUrn };
		var trustFinancialForecast = await _trustFinancialForecastGateway.GetById(getRequest, cancellationToken);

		if (trustFinancialForecast is null)
		{
			throw new NotFoundException($"Trust Financial Forecast with Id {request.TrustFinancialForecastId} not found");
		}

		if (trustFinancialForecast.ClosedAt.HasValue)
		{
			throw new StateChangeNotAllowedException($"Cannot update Trust Financial Forecast with Id {request.TrustFinancialForecastId} as it is closed.");
		}
	}
}