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

		var trustFinancialForecast = await GetTrustFinancialForecastToUpdate(request.TrustFinancialForecastId, cancellationToken);
		
		EnsureTrustFinancialForecastCanBeClosed(trustFinancialForecast);

		UpdateTrustFinancialForecastValues(request, trustFinancialForecast);
            
		return await _trustFinancialForecastGateway.Update(trustFinancialForecast, cancellationToken);
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

	private static void EnsureTrustFinancialForecastCanBeClosed(Data.Models.TrustFinancialForecast trustFinancialForecast)
	{
		if (trustFinancialForecast.ClosedAt.HasValue)
		{
			throw new StateChangeNotAllowedException($"Cannot close Trust Financial Forecast with Id {trustFinancialForecast.Id} as it is already closed.");
		}
	}
	
	private async Task<Data.Models.TrustFinancialForecast> GetTrustFinancialForecastToUpdate(int id, CancellationToken cancellationToken)
	{
		var trustFinancialForecast = await _trustFinancialForecastGateway.GetById(id, cancellationToken);

		if (trustFinancialForecast is null)
		{
			throw new NotFoundException($"Trust Financial Forecast with Id {id} not found");
		}

		return trustFinancialForecast;
	}
	
	private static void UpdateTrustFinancialForecastValues(CloseTrustFinancialForecastRequest request, Data.Models.TrustFinancialForecast trustFinancialForecast)
	{
		trustFinancialForecast.Notes = request.Notes;

		var now = DateTimeOffset.Now;
		trustFinancialForecast.ClosedAt = now;
		trustFinancialForecast.UpdatedAt = now;
	}
}