using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
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

	public async Task<TrustFinancialForecastResponse> GetById(int trustFinancialForecastId, CancellationToken cancellationToken = default)
	{
		var response = await _concernsDbContext
			.TrustFinancialForecasts
			.SingleAsync(f => f.Id == trustFinancialForecastId, cancellationToken);
		
		return response.ToResponseModel();
	}

	public Task<ICollection<TrustFinancialForecastResponse>> GetAllByCaseUrn(int caseUrn, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public async Task<int> Update(UpdateTrustFinancialForecastRequest updatedTrustFinancialForecastRequest, CancellationToken cancellationToken = default)
	{
		var trustFinancialForecast = await _concernsDbContext
			.TrustFinancialForecasts
			.SingleAsync(f => f.Id == updatedTrustFinancialForecastRequest.TrustFinancialForecastId, cancellationToken);
		
		trustFinancialForecast.CaseUrn = updatedTrustFinancialForecastRequest.CaseUrn;
		trustFinancialForecast.SRMAOfferedAfterTFF = updatedTrustFinancialForecastRequest.SRMAOfferedAfterTFF;
		trustFinancialForecast.ForecastingToolRanAt = updatedTrustFinancialForecastRequest.ForecastingToolRanAt;
		trustFinancialForecast.WasTrustResponseSatisfactory = updatedTrustFinancialForecastRequest.WasTrustResponseSatisfactory;
		trustFinancialForecast.Notes = updatedTrustFinancialForecastRequest.Notes;
		trustFinancialForecast.SFSOInitialReviewHappenedAt = updatedTrustFinancialForecastRequest.SFSOInitialReviewHappenedAt;
		trustFinancialForecast.TrustRespondedAt = updatedTrustFinancialForecastRequest.TrustRespondedAt;
		
		await _concernsDbContext.SaveChangesAsync(cancellationToken);
		
		return trustFinancialForecast.Id;
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
	
	public static TrustFinancialForecastResponse ToResponseModel(this TrustFinancialForecast trustFinancialForecast)
	{
		return new TrustFinancialForecastResponse
		{
			CaseUrn = trustFinancialForecast.CaseUrn,
			SRMAOfferedAfterTFF = trustFinancialForecast.SRMAOfferedAfterTFF,
			ForecastingToolRanAt = trustFinancialForecast.ForecastingToolRanAt,
			WasTrustResponseSatisfactory = trustFinancialForecast.WasTrustResponseSatisfactory,
			Notes = trustFinancialForecast.Notes,
			SFSOInitialReviewHappenedAt = trustFinancialForecast.SFSOInitialReviewHappenedAt,
			TrustRespondedAt = trustFinancialForecast.TrustRespondedAt
		};
	}
}
 	