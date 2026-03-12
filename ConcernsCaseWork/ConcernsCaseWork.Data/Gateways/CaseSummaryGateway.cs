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

		var financialPlans = await _concernsDbContext.FinancialPlanCases
			.Where(x => caseUrns.Contains(x.CaseUrn) && !x.ClosedAt.HasValue)
			.AsNoTracking()
			.ToListAsync();

		var ntisUnderConsideration = await _concernsDbContext.NTIUnderConsiderations
			.Where(x => caseUrns.Contains(x.CaseUrn) && !x.ClosedAt.HasValue)
			.AsNoTracking()
			.ToListAsync();

		var ntiWarningLetters = await _concernsDbContext.NTIWarningLetters
			.Where(x => caseUrns.Contains(x.CaseUrn) && !x.ClosedAt.HasValue)
			.AsNoTracking()
			.ToListAsync();

		var noticesToImprove = await _concernsDbContext.NoticesToImprove
			.Where(x => caseUrns.Contains(x.CaseUrn) && !x.ClosedAt.HasValue)
			.AsNoTracking()
			.ToListAsync();

		var srmaCases = await _concernsDbContext.SRMACases
			.Where(x => caseUrns.Contains(x.CaseUrn) && !x.ClosedAt.HasValue)
			.AsNoTracking()
			.ToListAsync();

		var trustFinancialForecasts = await _concernsDbContext.TrustFinancialForecasts
			.Where(x => caseUrns.Contains(x.CaseUrn) && !x.ClosedAt.HasValue)
			.AsNoTracking()
			.ToListAsync();

		var targetedTrustEngagements = await _concernsDbContext.TargetedTrustEngagements
			.Where(t => caseIds.Contains(t.CaseUrn) && !t.ClosedAt.HasValue)
			.Include(at => at.ActivityTypes)
			.AsNoTracking()
			.ToListAsync();

		var decisionsByCase = decisions.ToLookup(d => d.ConcernsCaseId);
		var financialPlansByUrn = financialPlans.ToLookup(x => x.CaseUrn);
		var ntisUCByUrn = ntisUnderConsideration.ToLookup(x => x.CaseUrn);
		var ntiWLByUrn = ntiWarningLetters.ToLookup(x => x.CaseUrn);
		var ntisByUrn = noticesToImprove.ToLookup(x => x.CaseUrn);
		var srmaByUrn = srmaCases.ToLookup(x => x.CaseUrn);
		var tffByUrn = trustFinancialForecasts.ToLookup(x => x.CaseUrn);
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
			FinancialPlanCases = financialPlansByUrn[c.Urn]
				.Select(x => new CaseSummaryVm.Action(x.CreatedAt, null, CaseSummaryConstants.FinancialPlan)).ToArray(),
			NtisUnderConsideration = ntisUCByUrn[c.Urn]
				.Select(x => new CaseSummaryVm.Action(x.CreatedAt, null, CaseSummaryConstants.NtiUnderConsideration)).ToArray(),
			NtiWarningLetters = ntiWLByUrn[c.Urn]
				.Select(x => new CaseSummaryVm.Action(x.CreatedAt, null, CaseSummaryConstants.NtiWarningLetter)).ToArray(),
			NoticesToImprove = ntisByUrn[c.Urn]
				.Select(x => new CaseSummaryVm.Action(x.CreatedAt, null, CaseSummaryConstants.Nti)).ToArray(),
			SrmaCases = srmaByUrn[c.Urn]
				.Select(x => new CaseSummaryVm.Action(x.CreatedAt, null, CaseSummaryConstants.Srma)).ToArray(),
			TrustFinancialForecasts = tffByUrn[c.Urn]
				.Select(x => new CaseSummaryVm.Action(x.CreatedAt.Date, null, CaseSummaryConstants.TrustFinancialForecast)).ToArray(),
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

		var financialPlans = await _concernsDbContext.FinancialPlanCases
			.Where(x => caseUrns.Contains(x.CaseUrn))
			.AsNoTracking()
			.ToListAsync();

		var ntisUnderConsideration = await _concernsDbContext.NTIUnderConsiderations
			.Where(x => caseUrns.Contains(x.CaseUrn))
			.AsNoTracking()
			.ToListAsync();

		var ntiWarningLetters = await _concernsDbContext.NTIWarningLetters
			.Where(x => caseUrns.Contains(x.CaseUrn))
			.AsNoTracking()
			.ToListAsync();

		var noticesToImprove = await _concernsDbContext.NoticesToImprove
			.Where(x => caseUrns.Contains(x.CaseUrn))
			.AsNoTracking()
			.ToListAsync();

		var srmaCases = await _concernsDbContext.SRMACases
			.Where(x => caseUrns.Contains(x.CaseUrn))
			.AsNoTracking()
			.ToListAsync();

		var trustFinancialForecasts = await _concernsDbContext.TrustFinancialForecasts
			.Where(x => caseUrns.Contains(x.CaseUrn))
			.AsNoTracking()
			.ToListAsync();

		var targetedTrustEngagements = await _concernsDbContext.TargetedTrustEngagements
			.Where(t => caseIds.Contains(t.CaseUrn) && t.ClosedAt.HasValue)
			.Include(at => at.ActivityTypes)
			.AsNoTracking()
			.ToListAsync();

		var decisionsByCase = decisions.ToLookup(d => d.ConcernsCaseId);
		var financialPlansByUrn = financialPlans.ToLookup(x => x.CaseUrn);
		var ntisUCByUrn = ntisUnderConsideration.ToLookup(x => x.CaseUrn);
		var ntiWLByUrn = ntiWarningLetters.ToLookup(x => x.CaseUrn);
		var ntisByUrn = noticesToImprove.ToLookup(x => x.CaseUrn);
		var srmaByUrn = srmaCases.ToLookup(x => x.CaseUrn);
		var tffByUrn = trustFinancialForecasts.ToLookup(x => x.CaseUrn);
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
			FinancialPlanCases = financialPlansByUrn[c.Urn]
				.Select(x => new CaseSummaryVm.Action(x.CreatedAt, x.ClosedAt, CaseSummaryConstants.FinancialPlan)).ToArray(),
			NtisUnderConsideration = ntisUCByUrn[c.Urn]
				.Select(x => new CaseSummaryVm.Action(x.CreatedAt, x.ClosedAt, CaseSummaryConstants.NtiUnderConsideration)).ToArray(),
			NtiWarningLetters = ntiWLByUrn[c.Urn]
				.Select(x => new CaseSummaryVm.Action(x.CreatedAt, x.ClosedAt, CaseSummaryConstants.NtiWarningLetter)).ToArray(),
			NoticesToImprove = ntisByUrn[c.Urn]
				.Select(x => new CaseSummaryVm.Action(x.CreatedAt, x.ClosedAt, CaseSummaryConstants.Nti)).ToArray(),
			SrmaCases = srmaByUrn[c.Urn]
				.Select(x => new CaseSummaryVm.Action(x.CreatedAt, x.ClosedAt, CaseSummaryConstants.Srma)).ToArray(),
			TrustFinancialForecasts = tffByUrn[c.Urn]
				.Select(x => new CaseSummaryVm.Action(x.CreatedAt.Date, x.ClosedAt?.DateTime, CaseSummaryConstants.TrustFinancialForecast)).ToArray(),
			TargetTrustEngagements = tteByCase[c.Id].ToArray(),
		}).ToList();
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
