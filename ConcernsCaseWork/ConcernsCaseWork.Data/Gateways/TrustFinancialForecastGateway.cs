using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Gateways;

public interface ITrustFinancialForecastGateway
{
	Task<TrustFinancialForecast> GetById(int trustFinancialForecastId, CancellationToken cancellationToken = default);
	Task<ICollection<TrustFinancialForecast>> GetAllForCase(int caseUrn, CancellationToken cancellationToken = default);
	Task<int> Update(TrustFinancialForecast trustFinancialForecast, CancellationToken cancellationToken = default);
	void Delete(int trustFinancialForecastId);
}
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
			.SingleOrDefaultAsync(f => f.Id == trustFinancialForecastId, cancellationToken);


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

	public void Delete(int trustFinancialForecastId)
	{
		var result = this._concernsDbContext.TrustFinancialForecasts.SingleOrDefault(f => f.Id == trustFinancialForecastId);
		result.DeletedAt = System.DateTime.Now;

		_concernsDbContext.SaveChanges();

	}
}