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

		var trustFinancialForecast = await GetTrustFinancialForecastToUpdate(request.TrustFinancialForecastId, cancellationToken);
		
		EnsureTrustFinancialForecastCanBeEdited(trustFinancialForecast);

		UpdateTrustFinancialForecastValues(request, trustFinancialForecast);
            
		return await _trustFinancialForecastGateway.Update(trustFinancialForecast, cancellationToken);
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

	private static void EnsureTrustFinancialForecastCanBeEdited(Data.Models.TrustFinancialForecast trustFinancialForecast)
	{
		if (trustFinancialForecast.ClosedAt.HasValue)
		{
			throw new StateChangeNotAllowedException($"Cannot update Trust Financial Forecast with Id {trustFinancialForecast.Id} as it is closed.");
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

	private static void UpdateTrustFinancialForecastValues(UpdateTrustFinancialForecastRequest request, Data.Models.TrustFinancialForecast trustFinancialForecast)
	{
		trustFinancialForecast.SRMAOfferedAfterTFF = request.SRMAOfferedAfterTFF;
		trustFinancialForecast.ForecastingToolRanAt = request.ForecastingToolRanAt;
		trustFinancialForecast.WasTrustResponseSatisfactory = request.WasTrustResponseSatisfactory;
		trustFinancialForecast.Notes = request.Notes;
		trustFinancialForecast.SFSOInitialReviewHappenedAt = request.SFSOInitialReviewHappenedAt;
		trustFinancialForecast.TrustRespondedAt = request.TrustRespondedAt;
		
		trustFinancialForecast.UpdatedAt = DateTimeOffset.Now;
	}
}