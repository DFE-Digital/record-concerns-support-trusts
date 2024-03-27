using Azure;
using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Data.Extensions;
using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Gateways;

public interface ICaseSummaryGateway
{
	Task<(IList<ActiveCaseSummaryVm>, int)> GetActiveCaseSummariesByOwner(GetCaseSummariesByOwnerParameters parameters);
	Task<(IList<ActiveCaseSummaryVm>, int)> GetActiveCaseSummariesByTeamMembers(GetCaseSummariesForUsersTeamParameters parameters);
	Task<(IList<ClosedCaseSummaryVm>, int)> GetClosedCaseSummariesByOwner(GetCaseSummariesByOwnerParameters parameters);
	Task<(IList<ActiveCaseSummaryVm>, int)> GetActiveCaseSummariesByTrust(GetCaseSummariesByTrustParameters parameters);
	Task<(IList<ClosedCaseSummaryVm>, int)> GetClosedCaseSummariesByTrust(GetCaseSummariesByTrustParameters parameters);
}

public class CaseSummaryGateway : ICaseSummaryGateway
{
	private readonly ConcernsDbContext _concernsDbContext;

	public CaseSummaryGateway(ConcernsDbContext concernsDbContext)
	{
		_concernsDbContext = concernsDbContext;
	}

	public async Task<(IList<ActiveCaseSummaryVm>, int)> GetActiveCaseSummariesByTeamMembers(GetCaseSummariesForUsersTeamParameters parameters )
	{
		var queryBuilder = _concernsDbContext.ConcernsCase
			.Include(cases => cases.Rating)
			.Include(cases => cases.Status)
			.Include(cases => cases.Decisions).ThenInclude(d => d.DecisionTypes)
			.Where(cases => parameters.teamMemberIds.Contains(cases.CreatedBy) && cases.Status.Name == "Live")
			.OrderByDescending(c => c.CreatedAt)
			.AsQueryable();

		var recordCount = queryBuilder.Count();

		if (parameters.Page.HasValue && parameters.Count.HasValue)
		{
			queryBuilder = queryBuilder.Paginate(parameters.Page.Value, parameters.Count.Value);
		}

		var cases = await SelectOpenCaseSummary(queryBuilder).ToListAsync();

		return (cases, recordCount);
	}

	public async Task<(IList<ActiveCaseSummaryVm>, int)> GetActiveCaseSummariesByOwner(GetCaseSummariesByOwnerParameters parameters)
	{
		var queryBuilder = _concernsDbContext.ConcernsCase
			.Include(cases => cases.Rating)
			.Include(cases => cases.Status)
			.Include(cases => cases.Decisions).ThenInclude(d => d.DecisionTypes)
			.Where(cases => cases.CreatedBy == parameters.Owner && cases.Status.Name == "Live")
			.OrderByDescending(c => c.CreatedAt)
			.AsQueryable();

		var recordCount = queryBuilder.Count();

		if (parameters.Page.HasValue && parameters.Count.HasValue)
		{
			queryBuilder = queryBuilder.Paginate(parameters.Page.Value, parameters.Count.Value);
		}

		var cases = await SelectOpenCaseSummary(queryBuilder).ToListAsync();

		return (cases, recordCount);
	}

	public async Task<(IList<ClosedCaseSummaryVm>, int)> GetClosedCaseSummariesByOwner(GetCaseSummariesByOwnerParameters parameters)
	{
		var queryBuilder = _concernsDbContext.ConcernsCase
			.Include(cases => cases.Rating)
			.Include(cases => cases.Decisions).ThenInclude(d => d.DecisionTypes)
			.Where(cases => cases.CreatedBy == parameters.Owner && cases.Status.Name == "Close")
			.OrderByDescending(c => c.CreatedAt)
			.AsQueryable();

		var recordCount = queryBuilder.Count();

		if (parameters.Page.HasValue && parameters.Count.HasValue)
		{
			queryBuilder = queryBuilder.Paginate(parameters.Page.Value, parameters.Count.Value);
		}

		var cases = await SelectClosedCaseSummary(queryBuilder).ToListAsync();

		return (cases, recordCount);
	}

	public async Task<(IList<ClosedCaseSummaryVm>, int)> GetClosedCaseSummariesByTrust(GetCaseSummariesByTrustParameters parameters)
	{
		var queryBuilder = _concernsDbContext.ConcernsCase
			.Include(cases => cases.Rating)
			.Include(cases => cases.Status)
			.Include(cases => cases.Decisions).ThenInclude(d => d.DecisionTypes)
			.Where(cases => cases.TrustUkprn == parameters.TrustUkPrn && cases.Status.Name == "Close")
			.OrderByDescending(c => c.CreatedAt)
			.AsQueryable();

		var recordCount = queryBuilder.Count();

		if (parameters.Page.HasValue && parameters.Count.HasValue)
		{
			queryBuilder = queryBuilder.Paginate(parameters.Page.Value, parameters.Count.Value);
		}

		var cases = await SelectClosedCaseSummary(queryBuilder).ToListAsync();

		return (cases, recordCount);
	}

	public async Task<(IList<ActiveCaseSummaryVm>, int)> GetActiveCaseSummariesByTrust(GetCaseSummariesByTrustParameters parameters)
	{
		var queryBuilder = _concernsDbContext.ConcernsCase
			.Include(cases => cases.Rating)
			.Include(cases => cases.Status)
			.Include(cases => cases.Decisions).ThenInclude(d => d.DecisionTypes)
			.Where(cases => cases.TrustUkprn == parameters.TrustUkPrn && cases.Status.Name == "Live")
			.OrderByDescending(c => c.CreatedAt)
			.AsQueryable();

		var recordCount = queryBuilder.Count();

		if (parameters.Page.HasValue && parameters.Count.HasValue)
		{
			queryBuilder = queryBuilder.Paginate(parameters.Page.Value, parameters.Count.Value);
		}

		var cases = await SelectOpenCaseSummary(queryBuilder).ToListAsync();

		return (cases, recordCount);
	}

	private IQueryable<ActiveCaseSummaryVm> SelectOpenCaseSummary(IQueryable<ConcernsCase> query)
	{
		var filteredQuery = ApplyCaseFilter(query);

		var data = filteredQuery.Select(cases => new ActiveCaseSummaryVm
		{
			CaseUrn = cases.Urn,
			CreatedAt = cases.CreatedAt,
			CreatedBy = cases.CreatedBy,
			Rating = cases.Rating,
			StatusName = cases.Status.Name,
			TrustUkPrn = cases.TrustUkprn,
			UpdatedAt = cases.UpdatedAt,
			CaseLastUpdatedAt = cases.CaseLastUpdatedAt,
			Division = cases.DivisionId,
			Region = cases.RegionId,
			Territory = cases.Territory,
			ActiveConcerns = from concerns
				in cases.ConcernsRecords
							 where concerns.StatusId == 1
							 select new CaseSummaryVm.Concern(concerns.ConcernsType.ToString(), concerns.ConcernsRating, concerns.CreatedAt),
			Decisions = from decisions
				in cases.Decisions
						where !decisions.ClosedAt.HasValue
						select decisions,
			FinancialPlanCases = _concernsDbContext.FinancialPlanCases
				.Where(x => x.CaseUrn == cases.Urn && !x.ClosedAt.HasValue)
				.Select(action => new CaseSummaryVm.Action(action.CreatedAt, null, "Action: Financial plan"))
				.ToArray(),
			NtisUnderConsideration = _concernsDbContext.NTIUnderConsiderations
				.Where(x => x.CaseUrn == cases.Urn && !x.ClosedAt.HasValue)
				.Select(action => new CaseSummaryVm.Action(action.CreatedAt, null, "Action: NTI under consideration"))
				.ToArray(),
			NtiWarningLetters = _concernsDbContext.NTIWarningLetters
				.Where(x => x.CaseUrn == cases.Urn && !x.ClosedAt.HasValue)
				.Select(action => new CaseSummaryVm.Action(action.CreatedAt, null, "Action: NTI warning letter"))
				.ToArray(),
			NoticesToImprove = _concernsDbContext.NoticesToImprove
				.Where(x => x.CaseUrn == cases.Urn && !x.ClosedAt.HasValue)
				.Select(action => new CaseSummaryVm.Action(action.CreatedAt, null, "Action: Notice To Improve"))
				.ToArray(),
			SrmaCases = _concernsDbContext.SRMACases
				.Where(x => x.CaseUrn == cases.Urn && !x.ClosedAt.HasValue)
				.Select(action => new CaseSummaryVm.Action(action.CreatedAt, null, "Action: School Resource Management Adviser"))
				.ToArray(),
			TrustFinancialForecasts = _concernsDbContext.TrustFinancialForecasts
				.Where(x => x.CaseUrn == cases.Urn && !x.ClosedAt.HasValue)
				.Select(action => new CaseSummaryVm.Action(action.CreatedAt.Date, null, CaseSummaryConstants.TrustFinancialForecast))
				.ToArray()
		});

		var result = ApplySplitQuery(data);

		return result;
	}

	private IQueryable<ClosedCaseSummaryVm> SelectClosedCaseSummary(IQueryable<ConcernsCase> query)
	{
		var filteredQuery = ApplyCaseFilter(query);

		var data = filteredQuery.Select(cases => new ClosedCaseSummaryVm
		{
			CaseUrn = cases.Urn,
			ClosedAt = cases.ClosedAt.Value,
			CreatedAt = cases.CreatedAt,
			CreatedBy = cases.CreatedBy,
			StatusName = cases.Status.Name,
			TrustUkPrn = cases.TrustUkprn,
			UpdatedAt = cases.UpdatedAt,
			Division = cases.DivisionId,
			Region = cases.RegionId,
			Territory = cases.Territory,
			ClosedConcerns =
							from concerns in cases.ConcernsRecords
							where concerns.StatusId == 3
							select new CaseSummaryVm.Concern(concerns.ConcernsType.ToString(), concerns.ConcernsRating, concerns.CreatedAt),
			Decisions = from decisions in cases.Decisions select decisions,
			FinancialPlanCases = _concernsDbContext.FinancialPlanCases
							.Where(x => x.CaseUrn == cases.Urn)
							.Select(action => new CaseSummaryVm.Action(action.CreatedAt, action.ClosedAt, "Action: Financial plan"))
							.ToArray(),
			NtisUnderConsideration = _concernsDbContext.NTIUnderConsiderations
							.Where(x => x.CaseUrn == cases.Urn)
							.Select(action => new CaseSummaryVm.Action(action.CreatedAt, action.ClosedAt, "Action: NTI under consideration"))
							.ToArray(),
			NtiWarningLetters = _concernsDbContext.NTIWarningLetters
							.Where(x => x.CaseUrn == cases.Urn)
							.Select(action => new CaseSummaryVm.Action(action.CreatedAt, action.ClosedAt, "Action: NTI warning letter"))
							.ToArray(),
			NoticesToImprove = _concernsDbContext.NoticesToImprove
							.Where(x => x.CaseUrn == cases.Urn)
							.Select(action => new CaseSummaryVm.Action(action.CreatedAt, action.ClosedAt, "Action: Notice To Improve"))
							.ToArray(),
			SrmaCases = _concernsDbContext.SRMACases
							.Where(x => x.CaseUrn == cases.Urn)
							.Select(action => new CaseSummaryVm.Action(action.CreatedAt, action.ClosedAt, "Action: School Resource Management Adviser"))
							.ToArray(),
			TrustFinancialForecasts = _concernsDbContext.TrustFinancialForecasts
							.Where(x => x.CaseUrn == cases.Urn)
							.Select(action => new CaseSummaryVm.Action(action.CreatedAt.Date, action.ClosedAt.Value.DateTime, CaseSummaryConstants.TrustFinancialForecast))
							.ToArray()
		});

		var result = ApplySplitQuery(data);

		return result;
	}

	private IQueryable<ConcernsCase> ApplyCaseFilter(IQueryable<ConcernsCase> query)
	{
		var caseIds = query.Select(c => c.Id).ToList();

		return _concernsDbContext.ConcernsCase
			.Include(cases => cases.Rating)
			.Include(cases => cases.Status)
			.Include(cases => cases.Decisions).ThenInclude(d => d.DecisionTypes)
			.Where(cases => caseIds.Contains(cases.Id));
	}

	private IQueryable<T> ApplySplitQuery<T>(IQueryable<T> query) where T : class
	{
		// In some cases getting the data as a split query can be beneficial
		// However as the number of records grow, having to query a large amount of records multiple times can give performance bottlenecks
		// Therefore even though we join on many tables, doing it in one hit can be faster
		// This is always tradeoffs
		var featureFlag = false;

		if (featureFlag)
		{
			return query;
		}

		// Some feature flag logic here
		return query.AsSplitQuery();
	}
}

public class GetCaseSummariesByTrustParameters
{
	public string TrustUkPrn { get; set; }
	public int? Page { get; set; }
	public int? Count { get; set; }
}

public class GetCaseSummariesByOwnerParameters
{
	public string Owner { get; set; }
	public int? Page { get; set; }
	public int? Count { get; set; }
}

public class GetCaseSummariesForUsersTeamParameters
{
	public string UserID { get; set; }
	public string[] teamMemberIds { get; set; }
	public int? Page { get; set; }
	public int? Count { get; set; }
}