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
		newTrustFinancialForecast.CreatedAt = DateTimeOffset.Now;
		newTrustFinancialForecast.UpdatedAt = DateTimeOffset.Now;
		
		_concernsDbContext.TrustFinancialForecasts.Add(newTrustFinancialForecast);
		await _concernsDbContext.SaveChangesAsync(cancellationToken);
		
		return newTrustFinancialForecast.Id;
	}

	public async Task<TrustFinancialForecastResponse> GetById(GetTrustFinancialForecastByIdRequest request, CancellationToken cancellationToken = default)
	{
		var response = await _concernsDbContext
			.TrustFinancialForecasts
			.SingleAsync(f => f.Id == request.TrustFinancialForecastId, cancellationToken);
		
		return response.ToResponseModel();
	}

	public async Task<ICollection<TrustFinancialForecastResponse>> GetAllForCase(GetTrustFinancialForecastForCaseRequest request, CancellationToken cancellationToken = default)
	{
		var query = _concernsDbContext.TrustFinancialForecasts
			.Where(x => x.CaseUrn == request.CaseUrn)
			.Select(action => action.ToResponseModel());
		
		return await query.ToArrayAsync(cancellationToken);
	}

	public async Task<int> Update(UpdateTrustFinancialForecastRequest request, CancellationToken cancellationToken = default)
	{
		var trustFinancialForecast = await _concernsDbContext
			.TrustFinancialForecasts
			.SingleAsync(f => f.Id == request.TrustFinancialForecastId, cancellationToken);
		
		trustFinancialForecast.SRMAOfferedAfterTFF = request.SRMAOfferedAfterTFF;
		trustFinancialForecast.ForecastingToolRanAt = request.ForecastingToolRanAt;
		trustFinancialForecast.WasTrustResponseSatisfactory = request.WasTrustResponseSatisfactory;
		trustFinancialForecast.Notes = request.Notes;
		trustFinancialForecast.SFSOInitialReviewHappenedAt = request.SFSOInitialReviewHappenedAt;
		trustFinancialForecast.TrustRespondedAt = request.TrustRespondedAt;
		trustFinancialForecast.UpdatedAt = DateTimeOffset.Now;
		
		await _concernsDbContext.SaveChangesAsync(cancellationToken);
		
		return trustFinancialForecast.Id;
	}

	public async Task<int> Close(CloseTrustFinancialForecastRequest request, CancellationToken cancellationToken = default)
	{
		var trustFinancialForecast = await _concernsDbContext
			.TrustFinancialForecasts
			.SingleAsync(f => f.Id == request.TrustFinancialForecastId, cancellationToken);
		
		trustFinancialForecast.Notes = request.Notes;
		trustFinancialForecast.ClosedAt = DateTimeOffset.Now;
		trustFinancialForecast.UpdatedAt = DateTimeOffset.Now;
		
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
			ClosedAt = trustFinancialForecast.ClosedAt,
			CreatedAt = trustFinancialForecast.CreatedAt,
			UpdatedAt = trustFinancialForecast.UpdatedAt,
			ForecastingToolRanAt = trustFinancialForecast.ForecastingToolRanAt,
			Notes = trustFinancialForecast.Notes,
			SFSOInitialReviewHappenedAt = trustFinancialForecast.SFSOInitialReviewHappenedAt,
			SRMAOfferedAfterTFF = trustFinancialForecast.SRMAOfferedAfterTFF,
			TrustRespondedAt = trustFinancialForecast.TrustRespondedAt,
			TrustFinancialForecastId = trustFinancialForecast.Id,
			WasTrustResponseSatisfactory = trustFinancialForecast.WasTrustResponseSatisfactory
		};
	}
}
 	