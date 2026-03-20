using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Data.Extensions;
using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Gateways;

public interface ICaseSummaryGateway
{
	Task<(IList<ActiveCaseSummaryVm>, int)> GetCaseSummariesByFilter(GetCaseSummariesByFilterParameters parameters);
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
			.Where(cases => parameters.teamMemberIds.Contains(cases.CreatedBy) && cases.Status.Name == "Live")
			.OrderByDescending(c => c.CreatedAt)
			.AsQueryable();

		var recordCount = queryBuilder.Count();

		if (parameters.Page.HasValue && parameters.Count.HasValue)
		{
			queryBuilder = queryBuilder.Paginate(parameters.Page.Value, parameters.Count.Value);
		}

		var caseIds = await queryBuilder.Select(c => c.Id).ToListAsync();

		var cases = await SelectOpenCaseSummary(caseIds).AsSplitQuery().ToListAsync();

		return (cases, recordCount);
	}

    public async Task<(IList<ActiveCaseSummaryVm>, int)> GetCaseSummariesByFilter(GetCaseSummariesByFilterParameters parameters)
    {
        var queryBuilder = _concernsDbContext.ConcernsCase
            .OrderByDescending(c => c.CreatedAt)
            .AsQueryable();

        if (parameters.Regions != null && parameters.Regions.Any())
        {
            var selectedRegions = parameters.Regions;
            var territoriesForSelectedRegions = GetTerritoriesForRegions(selectedRegions);
            queryBuilder = queryBuilder.Where(c =>
                (c.RegionId.HasValue && selectedRegions.Contains(c.RegionId.Value))
                || (c.Territory.HasValue && territoriesForSelectedRegions.Contains(c.Territory.Value)));
        }

		if (parameters.Owners != null && parameters.Owners.Any())
		{
			queryBuilder = queryBuilder.Where(c => parameters.Owners.Contains(c.CreatedBy));
		}

		if (parameters.TeamLeaders != null && parameters.TeamLeaders.Any())
		{
			queryBuilder = queryBuilder.Where(c => parameters.TeamLeaders.Contains(c.TeamLedBy));
		}

		if (parameters.Statuses != null && parameters.Statuses.Length > 0)
        {
			var statusIds = parameters.Statuses.Select(s => (int)s).ToArray();

			queryBuilder = queryBuilder.Where(c => statusIds.Contains(c.StatusId));
        }

        var recordCount = await queryBuilder.CountAsync();

        if (parameters.Page.HasValue && parameters.Count.HasValue)
        {
            queryBuilder = queryBuilder.Paginate(parameters.Page.Value, parameters.Count.Value);
        }

        var caseIds = await queryBuilder.Select(c => c.Id).ToListAsync();

        var cases = await SelectOpenCaseSummary(caseIds).AsSplitQuery().ToListAsync();

        return (cases, recordCount);
    }

	public async Task<(IList<ActiveCaseSummaryVm>, int)> GetActiveCaseSummariesByOwner(GetCaseSummariesByOwnerParameters parameters)
	{
		var queryBuilder = _concernsDbContext.ConcernsCase
			.Where(cases => cases.CreatedBy == parameters.Owner && cases.Status.Name == "Live")
			.OrderByDescending(c => c.CreatedAt)
			.AsQueryable();

		var recordCount = queryBuilder.Count();

		if (parameters.Page.HasValue && parameters.Count.HasValue)
		{
			queryBuilder = queryBuilder.Paginate(parameters.Page.Value, parameters.Count.Value);
		}

		var caseIds = await queryBuilder.Select(c => c.Id).ToListAsync();

		var cases = await SelectOpenCaseSummary(caseIds).AsSplitQuery().ToListAsync();

		return (cases, recordCount);
	}

	public async Task<(IList<ActiveCaseSummaryVm>, int)> GetActiveCaseSummariesByTrust(GetCaseSummariesByTrustParameters parameters)
	{
		var queryBuilder = _concernsDbContext.ConcernsCase
			.Where(cases => cases.TrustUkprn == parameters.TrustUkPrn && cases.Status.Name == "Live")
			.OrderByDescending(c => c.CreatedAt)
			.AsQueryable();

		var recordCount = queryBuilder.Count();

		if (parameters.Page.HasValue && parameters.Count.HasValue)
		{
			queryBuilder = queryBuilder.Paginate(parameters.Page.Value, parameters.Count.Value);
		}

		var caseIds = await queryBuilder.Select(c => c.Id).ToListAsync();

		var cases = await SelectOpenCaseSummary(caseIds).AsSplitQuery().ToListAsync();

		return (cases, recordCount);
	}

	public async Task<(IList<ClosedCaseSummaryVm>, int)> GetClosedCaseSummariesByOwner(GetCaseSummariesByOwnerParameters parameters)
	{
		var queryBuilder = _concernsDbContext.ConcernsCase
			.Where(cases => cases.CreatedBy == parameters.Owner && cases.Status.Name == "Close")
			.OrderByDescending(c => c.CreatedAt)
			.AsQueryable();

		var recordCount = queryBuilder.Count();

		if (parameters.Page.HasValue && parameters.Count.HasValue)
		{
			queryBuilder = queryBuilder.Paginate(parameters.Page.Value, parameters.Count.Value);
		}

		var caseIds = await queryBuilder.Select(c => c.Id).ToListAsync();

		var cases = await SelectClosedCaseSummary(caseIds).AsSplitQuery().ToListAsync();

		return (cases, recordCount);
	}

	public async Task<(IList<ClosedCaseSummaryVm>, int)> GetClosedCaseSummariesByTrust(GetCaseSummariesByTrustParameters parameters)
	{
		var queryBuilder = _concernsDbContext.ConcernsCase
			.Where(cases => cases.TrustUkprn == parameters.TrustUkPrn && cases.Status.Name == "Close")
			.OrderByDescending(c => c.CreatedAt)
			.AsQueryable();

		var recordCount = queryBuilder.Count();

		if (parameters.Page.HasValue && parameters.Count.HasValue)
		{
			queryBuilder = queryBuilder.Paginate(parameters.Page.Value, parameters.Count.Value);
		}

		var caseIds = await queryBuilder.Select(c => c.Id).ToListAsync();

		var cases = await SelectClosedCaseSummary(caseIds).AsSplitQuery().ToListAsync();

		return (cases, recordCount);
	}

	private IOrderedQueryable<ConcernsCase> GetCases(List<int> caseIds)
	{
		return _concernsDbContext.ConcernsCase.Where(c => caseIds.Contains(c.Id)).OrderByDescending(c => c.CreatedAt);
	}

	private IQueryable<ActiveCaseSummaryVm> SelectOpenCaseSummary(List<int> caseIds)
	{
		var query = GetCases(caseIds);

		var result = SelectOpenCaseSummary(query);

		return result;
	}

	private IQueryable<ActiveCaseSummaryVm> SelectOpenCaseSummary(IQueryable<ConcernsCase> query)
	{
		return query.Select(cases => new ActiveCaseSummaryVm
		{
			CaseUrn = cases.Urn,
			CreatedAt = cases.CreatedAt,
			CreatedBy = cases.CreatedBy,
			TeamLedBy = cases.TeamLedBy,
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
			Decisions = _concernsDbContext.Decisions.Where(d => d.ConcernsCaseId == cases.Id && !d.ClosedAt.HasValue).Include(d => d.DecisionTypes).ToArray(),
			FinancialPlanCases = _concernsDbContext.FinancialPlanCases
				.Where(x => x.CaseUrn == cases.Urn && !x.ClosedAt.HasValue)
				.Select(action => new CaseSummaryVm.Action(action.CreatedAt, null, CaseSummaryConstants.FinancialPlan))
				.ToArray(),
			NtisUnderConsideration = _concernsDbContext.NTIUnderConsiderations
				.Where(x => x.CaseUrn == cases.Urn && !x.ClosedAt.HasValue)
				.Select(action => new CaseSummaryVm.Action(action.CreatedAt, null, CaseSummaryConstants.NtiUnderConsideration))
				.ToArray(),
			NtiWarningLetters = _concernsDbContext.NTIWarningLetters
				.Where(x => x.CaseUrn == cases.Urn && !x.ClosedAt.HasValue)
				.Select(action => new CaseSummaryVm.Action(action.CreatedAt, null, CaseSummaryConstants.NtiWarningLetter))
				.ToArray(),
			NoticesToImprove = _concernsDbContext.NoticesToImprove
				.Where(x => x.CaseUrn == cases.Urn && !x.ClosedAt.HasValue)
				.Select(action => new CaseSummaryVm.Action(action.CreatedAt, null, CaseSummaryConstants.Nti))
				.ToArray(),
			SrmaCases = _concernsDbContext.SRMACases
				.Where(x => x.CaseUrn == cases.Urn && !x.ClosedAt.HasValue)
				.Select(action => new CaseSummaryVm.Action(action.CreatedAt, null, CaseSummaryConstants.Srma))
				.ToArray(),
			TrustFinancialForecasts = _concernsDbContext.TrustFinancialForecasts
				.Where(x => x.CaseUrn == cases.Urn && !x.ClosedAt.HasValue)
				.Select(action => new CaseSummaryVm.Action(action.CreatedAt.Date, null, CaseSummaryConstants.TrustFinancialForecast))
				.ToArray(),
			TargetTrustEngagements = _concernsDbContext.TargetedTrustEngagements.Where(t => t.CaseUrn == cases.Id && !t.ClosedAt.HasValue).Include(at => at.ActivityTypes).ToArray(),
		});
	}

	private IQueryable<ClosedCaseSummaryVm> SelectClosedCaseSummary(List<int> caseIds)
	{
		var query = GetCases(caseIds);

		var result = SelectClosedCaseSummary(query);

		return result;
	}

	private static Territory[] GetTerritoriesForRegions(IReadOnlyList<Region> regions)
	{
		var set = new HashSet<Territory>();
		foreach (var region in regions)
		{
			foreach (var territory in MapRegionToTerritories(region))
			{
				set.Add(territory);
			}
		}

		return set.ToArray();
	}

	private static IEnumerable<Territory> MapRegionToTerritories(Region region)
	{
		return region switch
		{
			Region.EastMidlands => new[] { Territory.Midlands_And_West__East_Midlands },
			Region.EastOfEngland => new[] { Territory.South_And_South_East__East_Of_England },
			Region.London => new[] { Territory.South_And_South_East__London },
			Region.NorthEast => new[] { Territory.North_And_Utc__North_East },
			Region.NorthWest => new[] { Territory.North_And_Utc__North_West },
			Region.SouthEast => new[] { Territory.South_And_South_East__South_East },
			Region.SouthWest => new[] { Territory.Midlands_And_West__SouthWest },
			Region.WestMidlands => new[] { Territory.Midlands_And_West__West_Midlands },
			Region.YorkshireAndTheHumber => new[] { Territory.North_And_Utc__Yorkshire_And_Humber },
			_ => Array.Empty<Territory>()
		};
	}

	private IQueryable<ClosedCaseSummaryVm> SelectClosedCaseSummary(IQueryable<ConcernsCase> query)
	{
		return query.Select(cases => new ClosedCaseSummaryVm
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
			Decisions = _concernsDbContext.Decisions.Where(d => d.ConcernsCaseId == cases.Id && d.ClosedAt.HasValue).Include(d => d.DecisionTypes).ToArray(),
			FinancialPlanCases = _concernsDbContext.FinancialPlanCases
							.Where(x => x.CaseUrn == cases.Urn)
							.Select(action => new CaseSummaryVm.Action(action.CreatedAt, action.ClosedAt, CaseSummaryConstants.FinancialPlan))
							.ToArray(),
			NtisUnderConsideration = _concernsDbContext.NTIUnderConsiderations
							.Where(x => x.CaseUrn == cases.Urn)
							.Select(action => new CaseSummaryVm.Action(action.CreatedAt, action.ClosedAt, CaseSummaryConstants.NtiUnderConsideration))
							.ToArray(),
			NtiWarningLetters = _concernsDbContext.NTIWarningLetters
							.Where(x => x.CaseUrn == cases.Urn)
							.Select(action => new CaseSummaryVm.Action(action.CreatedAt, action.ClosedAt, CaseSummaryConstants.NtiWarningLetter))
							.ToArray(),
			NoticesToImprove = _concernsDbContext.NoticesToImprove
							.Where(x => x.CaseUrn == cases.Urn)
							.Select(action => new CaseSummaryVm.Action(action.CreatedAt, action.ClosedAt, CaseSummaryConstants.Nti))
							.ToArray(),
			SrmaCases = _concernsDbContext.SRMACases
							.Where(x => x.CaseUrn == cases.Urn)
							.Select(action => new CaseSummaryVm.Action(action.CreatedAt, action.ClosedAt, CaseSummaryConstants.Srma))
							.ToArray(),
			TrustFinancialForecasts = _concernsDbContext.TrustFinancialForecasts
							.Where(x => x.CaseUrn == cases.Urn)
							.Select(action => new CaseSummaryVm.Action(action.CreatedAt.Date, action.ClosedAt.Value.DateTime, CaseSummaryConstants.TrustFinancialForecast))
							.ToArray(),
			TargetTrustEngagements = _concernsDbContext.TargetedTrustEngagements.Where(t => t.CaseUrn == cases.Id && t.ClosedAt.HasValue).Include(at => at.ActivityTypes).ToArray(),
		});
	}
}

public class GetCaseSummariesByTrustParameters
{
	public string TrustUkPrn { get; set; }
	public int? Page { get; set; }
	public int? Count { get; set; }
}

public class GetCaseSummariesByFilterParameters
{
	public Region[] Regions { get; set; }
	public string[] Owners { get; set; }
	public string[] TeamLeaders { get; set; }
	public CaseStatus[] Statuses { get; set; }
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