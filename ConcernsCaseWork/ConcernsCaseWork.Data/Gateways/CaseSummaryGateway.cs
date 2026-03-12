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

	public async Task<(IList<ActiveCaseSummaryVm>, int)> GetActiveCaseSummariesByTeamMembers(GetCaseSummariesForUsersTeamParameters parameters)
	{
		var queryBuilder = _concernsDbContext.ConcernsCase
			.Where(cases => parameters.teamMemberIds.Contains(cases.CreatedBy) && cases.Status.Name == "Live")
			.OrderByDescending(c => c.CreatedAt)
			.AsQueryable();

		var recordCount = await queryBuilder.CountAsync();

		if (parameters.Page.HasValue && parameters.Count.HasValue)
		{
			queryBuilder = queryBuilder.Paginate(parameters.Page.Value, parameters.Count.Value);
		}

		var caseIds = await queryBuilder.Select(c => c.Id).ToListAsync();
		var cases = await BuildActiveCaseSummaries(caseIds);

		return (cases, recordCount);
	}

	public async Task<(IList<ActiveCaseSummaryVm>, int)> GetActiveCaseSummariesByOwner(GetCaseSummariesByOwnerParameters parameters)
	{
		var queryBuilder = _concernsDbContext.ConcernsCase
			.Where(cases => cases.CreatedBy == parameters.Owner && cases.Status.Name == "Live")
			.OrderByDescending(c => c.CreatedAt)
			.AsQueryable();

		var recordCount = await queryBuilder.CountAsync();

		if (parameters.Page.HasValue && parameters.Count.HasValue)
		{
			queryBuilder = queryBuilder.Paginate(parameters.Page.Value, parameters.Count.Value);
		}

		var caseIds = await queryBuilder.Select(c => c.Id).ToListAsync();
		var cases = await BuildActiveCaseSummaries(caseIds);

		return (cases, recordCount);
	}

	public async Task<(IList<ActiveCaseSummaryVm>, int)> GetActiveCaseSummariesByTrust(GetCaseSummariesByTrustParameters parameters)
	{
		var queryBuilder = _concernsDbContext.ConcernsCase
			.Where(cases => cases.TrustUkprn == parameters.TrustUkPrn && cases.Status.Name == "Live")
			.OrderByDescending(c => c.CreatedAt)
			.AsQueryable();

		var recordCount = await queryBuilder.CountAsync();

		if (parameters.Page.HasValue && parameters.Count.HasValue)
		{
			queryBuilder = queryBuilder.Paginate(parameters.Page.Value, parameters.Count.Value);
		}

		var caseIds = await queryBuilder.Select(c => c.Id).ToListAsync();
		var cases = await BuildActiveCaseSummaries(caseIds);

		return (cases, recordCount);
	}

	public async Task<(IList<ClosedCaseSummaryVm>, int)> GetClosedCaseSummariesByOwner(GetCaseSummariesByOwnerParameters parameters)
	{
		var queryBuilder = _concernsDbContext.ConcernsCase
			.Where(cases => cases.CreatedBy == parameters.Owner && cases.Status.Name == "Close")
			.OrderByDescending(c => c.CreatedAt)
			.AsQueryable();

		var recordCount = await queryBuilder.CountAsync();

		if (parameters.Page.HasValue && parameters.Count.HasValue)
		{
			queryBuilder = queryBuilder.Paginate(parameters.Page.Value, parameters.Count.Value);
		}

		var caseIds = await queryBuilder.Select(c => c.Id).ToListAsync();
		var cases = await BuildClosedCaseSummaries(caseIds);

		return (cases, recordCount);
	}

	public async Task<(IList<ClosedCaseSummaryVm>, int)> GetClosedCaseSummariesByTrust(GetCaseSummariesByTrustParameters parameters)
	{
		var queryBuilder = _concernsDbContext.ConcernsCase
			.Where(cases => cases.TrustUkprn == parameters.TrustUkPrn && cases.Status.Name == "Close")
			.OrderByDescending(c => c.CreatedAt)
			.AsQueryable();

		var recordCount = await queryBuilder.CountAsync();

		if (parameters.Page.HasValue && parameters.Count.HasValue)
		{
			queryBuilder = queryBuilder.Paginate(parameters.Page.Value, parameters.Count.Value);
		}

		var caseIds = await queryBuilder.Select(c => c.Id).ToListAsync();
		var cases = await BuildClosedCaseSummaries(caseIds);

		return (cases, recordCount);
	}

	private async Task<List<ActiveCaseSummaryVm>> BuildActiveCaseSummaries(List<int> caseIds)
	{
		if (caseIds.Count == 0)
			return new List<ActiveCaseSummaryVm>();

		var cases = await _concernsDbContext.ConcernsCase
			.Where(c => caseIds.Contains(c.Id))
			.Include(c => c.Rating)
			.Include(c => c.Status)
			.Include(c => c.ConcernsRecords).ThenInclude(cr => cr.ConcernsType)
			.Include(c => c.ConcernsRecords).ThenInclude(cr => cr.ConcernsRating)
			.OrderByDescending(c => c.CreatedAt)
			.AsSplitQuery()
			.AsNoTracking()
			.ToListAsync();

		var caseUrns = cases.Select(c => c.Urn).ToList();

		var decisions = await _concernsDbContext.Decisions
			.Where(d => caseIds.Contains(d.ConcernsCaseId) && !d.ClosedAt.HasValue)
			.Include(d => d.DecisionTypes)
			.AsNoTracking()
			.ToListAsync();

		var actions = await GetActionsForCases(caseUrns, openOnly: true);

		var targetedTrustEngagements = await _concernsDbContext.TargetedTrustEngagements
			.Where(t => caseIds.Contains(t.CaseUrn) && !t.ClosedAt.HasValue)
			.Include(at => at.ActivityTypes)
			.AsNoTracking()
			.ToListAsync();

		var decisionsByCase = decisions.ToLookup(d => d.ConcernsCaseId);
		var actionsByUrn = actions.ToLookup(a => a.CaseUrn);
		var tteByCase = targetedTrustEngagements.ToLookup(t => t.CaseUrn);

		return cases.Select(c => new ActiveCaseSummaryVm
		{
			CaseUrn = c.Urn,
			CreatedAt = c.CreatedAt,
			CreatedBy = c.CreatedBy,
			Rating = c.Rating,
			StatusName = c.Status.Name,
			TrustUkPrn = c.TrustUkprn,
			UpdatedAt = c.UpdatedAt,
			CaseLastUpdatedAt = c.CaseLastUpdatedAt,
			Division = c.DivisionId,
			Region = c.RegionId,
			Territory = c.Territory,
			ActiveConcerns = c.ConcernsRecords
				.Where(cr => cr.StatusId == 1)
				.Select(cr => new CaseSummaryVm.Concern(cr.ConcernsType.ToString(), cr.ConcernsRating, cr.CreatedAt)),
			Decisions = decisionsByCase[c.Id].ToArray(),
			FinancialPlanCases = GetActionsForCase(actionsByUrn, c.Urn, CaseSummaryConstants.FinancialPlan),
			NtisUnderConsideration = GetActionsForCase(actionsByUrn, c.Urn, CaseSummaryConstants.NtiUnderConsideration),
			NtiWarningLetters = GetActionsForCase(actionsByUrn, c.Urn, CaseSummaryConstants.NtiWarningLetter),
			NoticesToImprove = GetActionsForCase(actionsByUrn, c.Urn, CaseSummaryConstants.Nti),
			SrmaCases = GetActionsForCase(actionsByUrn, c.Urn, CaseSummaryConstants.Srma),
			TrustFinancialForecasts = GetActionsForCase(actionsByUrn, c.Urn, CaseSummaryConstants.TrustFinancialForecast),
			TargetTrustEngagements = tteByCase[c.Id].ToArray(),
		}).ToList();
	}

	private async Task<List<ClosedCaseSummaryVm>> BuildClosedCaseSummaries(List<int> caseIds)
	{
		if (caseIds.Count == 0)
			return new List<ClosedCaseSummaryVm>();

		var cases = await _concernsDbContext.ConcernsCase
			.Where(c => caseIds.Contains(c.Id))
			.Include(c => c.Status)
			.Include(c => c.ConcernsRecords).ThenInclude(cr => cr.ConcernsType)
			.Include(c => c.ConcernsRecords).ThenInclude(cr => cr.ConcernsRating)
			.OrderByDescending(c => c.CreatedAt)
			.AsSplitQuery()
			.AsNoTracking()
			.ToListAsync();

		var caseUrns = cases.Select(c => c.Urn).ToList();

		var decisions = await _concernsDbContext.Decisions
			.Where(d => caseIds.Contains(d.ConcernsCaseId) && d.ClosedAt.HasValue)
			.Include(d => d.DecisionTypes)
			.AsNoTracking()
			.ToListAsync();

		var actions = await GetActionsForCases(caseUrns, openOnly: false);

		var targetedTrustEngagements = await _concernsDbContext.TargetedTrustEngagements
			.Where(t => caseIds.Contains(t.CaseUrn) && t.ClosedAt.HasValue)
			.Include(at => at.ActivityTypes)
			.AsNoTracking()
			.ToListAsync();

		var decisionsByCase = decisions.ToLookup(d => d.ConcernsCaseId);
		var actionsByUrn = actions.ToLookup(a => a.CaseUrn);
		var tteByCase = targetedTrustEngagements.ToLookup(t => t.CaseUrn);

		return cases.Select(c => new ClosedCaseSummaryVm
		{
			CaseUrn = c.Urn,
			ClosedAt = c.ClosedAt.Value,
			CreatedAt = c.CreatedAt,
			CreatedBy = c.CreatedBy,
			StatusName = c.Status.Name,
			TrustUkPrn = c.TrustUkprn,
			UpdatedAt = c.UpdatedAt,
			Division = c.DivisionId,
			Region = c.RegionId,
			Territory = c.Territory,
			ClosedConcerns = c.ConcernsRecords
				.Where(cr => cr.StatusId == 3)
				.Select(cr => new CaseSummaryVm.Concern(cr.ConcernsType.ToString(), cr.ConcernsRating, cr.CreatedAt)),
			Decisions = decisionsByCase[c.Id].ToArray(),
			FinancialPlanCases = GetActionsForCase(actionsByUrn, c.Urn, CaseSummaryConstants.FinancialPlan),
			NtisUnderConsideration = GetActionsForCase(actionsByUrn, c.Urn, CaseSummaryConstants.NtiUnderConsideration),
			NtiWarningLetters = GetActionsForCase(actionsByUrn, c.Urn, CaseSummaryConstants.NtiWarningLetter),
			NoticesToImprove = GetActionsForCase(actionsByUrn, c.Urn, CaseSummaryConstants.Nti),
			SrmaCases = GetActionsForCase(actionsByUrn, c.Urn, CaseSummaryConstants.Srma),
			TrustFinancialForecasts = GetActionsForCase(actionsByUrn, c.Urn, CaseSummaryConstants.TrustFinancialForecast),
			TargetTrustEngagements = tteByCase[c.Id].ToArray(),
		}).ToList();
	}

	private async Task<List<ActionRecord>> GetActionsForCases(List<int> caseUrns, bool openOnly)
	{
		var query = _concernsDbContext.FinancialPlanCases.Where(x => caseUrns.Contains(x.CaseUrn));
		if (openOnly) query = query.Where(x => !x.ClosedAt.HasValue);
		var combined = query
			.Select(x => new ActionRecord { CaseUrn = x.CaseUrn, CreatedAt = x.CreatedAt, ClosedAt = openOnly ? null : x.ClosedAt, Name = CaseSummaryConstants.FinancialPlan });

		var ntiUC = _concernsDbContext.NTIUnderConsiderations.Where(x => caseUrns.Contains(x.CaseUrn));
		if (openOnly) ntiUC = ntiUC.Where(x => !x.ClosedAt.HasValue);
		combined = combined.Concat(ntiUC
			.Select(x => new ActionRecord { CaseUrn = x.CaseUrn, CreatedAt = x.CreatedAt, ClosedAt = openOnly ? null : x.ClosedAt, Name = CaseSummaryConstants.NtiUnderConsideration }));

		var ntiWL = _concernsDbContext.NTIWarningLetters.Where(x => caseUrns.Contains(x.CaseUrn));
		if (openOnly) ntiWL = ntiWL.Where(x => !x.ClosedAt.HasValue);
		combined = combined.Concat(ntiWL
			.Select(x => new ActionRecord { CaseUrn = x.CaseUrn, CreatedAt = x.CreatedAt, ClosedAt = openOnly ? null : x.ClosedAt, Name = CaseSummaryConstants.NtiWarningLetter }));

		var nti = _concernsDbContext.NoticesToImprove.Where(x => caseUrns.Contains(x.CaseUrn));
		if (openOnly) nti = nti.Where(x => !x.ClosedAt.HasValue);
		combined = combined.Concat(nti
			.Select(x => new ActionRecord { CaseUrn = x.CaseUrn, CreatedAt = x.CreatedAt, ClosedAt = openOnly ? null : x.ClosedAt, Name = CaseSummaryConstants.Nti }));

		var srma = _concernsDbContext.SRMACases.Where(x => caseUrns.Contains(x.CaseUrn));
		if (openOnly) srma = srma.Where(x => !x.ClosedAt.HasValue);
		combined = combined.Concat(srma
			.Select(x => new ActionRecord { CaseUrn = x.CaseUrn, CreatedAt = x.CreatedAt, ClosedAt = openOnly ? null : x.ClosedAt, Name = CaseSummaryConstants.Srma }));

		var actions = await combined.AsNoTracking().ToListAsync();

		var tffQuery = _concernsDbContext.TrustFinancialForecasts.Where(x => caseUrns.Contains(x.CaseUrn));
		if (openOnly) tffQuery = tffQuery.Where(x => !x.ClosedAt.HasValue);
		var tffActions = await tffQuery
			.Select(x => new { x.CaseUrn, x.CreatedAt, x.ClosedAt })
			.AsNoTracking()
			.ToListAsync();
		actions.AddRange(tffActions.Select(x => new ActionRecord
		{
			CaseUrn = x.CaseUrn,
			CreatedAt = x.CreatedAt.Date,
			ClosedAt = x.ClosedAt?.DateTime,
			Name = CaseSummaryConstants.TrustFinancialForecast
		}));

		return actions;
	}

	private static CaseSummaryVm.Action[] GetActionsForCase(ILookup<int, ActionRecord> actionsByUrn, int caseUrn, string actionName)
	{
		return actionsByUrn[caseUrn]
			.Where(a => a.Name == actionName)
			.Select(a => new CaseSummaryVm.Action(a.CreatedAt, a.ClosedAt, a.Name))
			.ToArray();
	}

	private class ActionRecord
	{
		public int CaseUrn { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? ClosedAt { get; set; }
		public string Name { get; set; }
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
