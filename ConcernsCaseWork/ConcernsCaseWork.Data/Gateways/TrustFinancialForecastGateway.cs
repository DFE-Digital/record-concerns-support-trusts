using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Gateways;

public class TrustFinancialForecastGateway : ITrustFinancialForecastGateway
{
	private readonly ConcernsDbContext _concernsDbContext;

	public TrustFinancialForecastGateway(ConcernsDbContext concernsDbContext)
	{
		_concernsDbContext = concernsDbContext;
	}

	public async Task<int> Create(CreateTrustFinancialForecastRequest trustFinancialForecast, CancellationToken cancellationToken = default)
	{
		var newTrustFinancialForecast = trustFinancialForecast.ToDbModel();
		_concernsDbContext.TrustFinancialForecasts.Add(newTrustFinancialForecast);
		await _concernsDbContext.SaveChangesAsync(cancellationToken);
		return newTrustFinancialForecast.Id;
	}

	public async Task<TrustFinancialForecast> GetById(int trustFinancialForecastId, CancellationToken cancellationToken = default)
		=> await _concernsDbContext.TrustFinancialForecasts.SingleAsync(f => f.Id == trustFinancialForecastId, cancellationToken);

	public Task<ICollection<TrustFinancialForecast>> GetAllByCaseUrn(int caseUrn, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task Update(UpdateTrustFinancialForecastRequest updatedTrustFinancialForecastRequest, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}

public static class TrustFinancialForecastExtensions
{
	public static TrustFinancialForecast ToDbModel(this CreateTrustFinancialForecastRequest createTrustFinancialForecastRequest)
	{
		return new TrustFinancialForecast
		{
			CaseUrn = createTrustFinancialForecastRequest.CaseUrn,
			SRMAOfferedAfterTFF = createTrustFinancialForecastRequest.SRMAOfferedAfterTFF,
			ForecastingToolRanAt = createTrustFinancialForecastRequest.ForecastingToolRanAt,
			WasTrustResponseSatisfactory = createTrustFinancialForecastRequest.WasTrustResponseSatisfactory,
			Notes = createTrustFinancialForecastRequest.Notes,
			SFSOInitialReviewHappenedAt = createTrustFinancialForecastRequest.SFSOInitialReviewHappenedAt,
			TrustRespondedAt = createTrustFinancialForecastRequest.TrustRespondedAt
		};
	}
}
 	