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

	public async Task<TrustFinancialForecast> GetById(int trustFinancialForecastId, CancellationToken cancellationToken = default)
		=> await _concernsDbContext
			.TrustFinancialForecasts
			.SingleAsync(f => f.Id == trustFinancialForecastId, cancellationToken);


	public async Task<ICollection<TrustFinancialForecast>> GetAllForCase(int caseUrn, CancellationToken cancellationToken = default)
	{
		var query = _concernsDbContext
			.TrustFinancialForecasts
			.Where(x => x.CaseUrn == caseUrn);
		
		return await query.ToArrayAsync(cancellationToken);
	}

	public async Task<int> Update(TrustFinancialForecast trustFinancialForecast, CancellationToken cancellationToken = default)
	{
		_concernsDbContext.TrustFinancialForecasts.Update(trustFinancialForecast);
		
		await _concernsDbContext.SaveChangesAsync(cancellationToken);
		
		return trustFinancialForecast.Id;
	}
}