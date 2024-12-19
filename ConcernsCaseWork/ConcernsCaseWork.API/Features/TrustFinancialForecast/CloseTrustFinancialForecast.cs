using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.TrustFinancialForecast;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Exceptions;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;
using NotFoundException = ConcernsCaseWork.API.Exceptions.NotFoundException;

namespace ConcernsCaseWork.API.Features.TrustFinancialForecast;

public class CloseTrustFinancialForecast(IConcernsCaseGateway concernsCaseGateway, ITrustFinancialForecastGateway trustFinancialForecastGateway) : IUseCaseAsync<CloseTrustFinancialForecastRequest, int>
{
	private readonly IConcernsCaseGateway _concernsCaseGateway = Guard.Against.Null(concernsCaseGateway);
	private readonly ITrustFinancialForecastGateway _trustFinancialForecastGateway = Guard.Against.Null(trustFinancialForecastGateway);

	public async Task<int> Execute(CloseTrustFinancialForecastRequest request, CancellationToken cancellationToken)
	{
		EnsureRequestIsValid(request);

		var cc = GetCase(request.CaseUrn);

		var trustFinancialForecast = await GetTrustFinancialForecastToUpdate(request.TrustFinancialForecastId, cancellationToken);

		EnsureTrustFinancialForecastCanBeClosed(trustFinancialForecast);

		UpdateTrustFinancialForecastValues(request, trustFinancialForecast);

		var result = await _trustFinancialForecastGateway.Update(trustFinancialForecast, cancellationToken);

		cc.CaseLastUpdatedAt = trustFinancialForecast.UpdatedAt.DateTime;

		await _concernsCaseGateway.UpdateExistingAsync(cc);

		return result;
	}

	private static void EnsureRequestIsValid(CloseTrustFinancialForecastRequest request)
	{
		if (request is null)
			throw new ArgumentNullException(nameof(request));

		if (!request.IsValid())
			throw new ArgumentException("Request is not valid", nameof(request));
	}

	private ConcernsCase GetCase(int caseUrn)
	{
		var cc = _concernsCaseGateway.GetConcernsCaseByUrn(caseUrn);
		if (cc == null)
			throw new NotFoundException($"Concerns Case {caseUrn} not found");
		return cc;
	}

	private static void EnsureTrustFinancialForecastCanBeClosed(Data.Models.TrustFinancialForecast trustFinancialForecast)
	{
		if (trustFinancialForecast.ClosedAt.HasValue)
			throw new StateChangeNotAllowedException($"Cannot close Trust Financial Forecast with Id {trustFinancialForecast.Id} as it is already closed.");
	}

	private async Task<Data.Models.TrustFinancialForecast> GetTrustFinancialForecastToUpdate(int id, CancellationToken cancellationToken)
	{
		var trustFinancialForecast = await _trustFinancialForecastGateway.GetById(id, cancellationToken);

		if (trustFinancialForecast is null)
			throw new NotFoundException($"Trust Financial Forecast with Id {id} not found");

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