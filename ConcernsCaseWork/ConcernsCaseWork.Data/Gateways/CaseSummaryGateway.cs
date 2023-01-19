using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Gateways;

public class CaseSummaryGateway : ICaseSummaryGateway
{
	private readonly ConcernsDbContext _concernsDbContext;

	public CaseSummaryGateway(ConcernsDbContext concernsDbContext)
	{
		_concernsDbContext = concernsDbContext;
	}

	public async Task<IList<ActiveCaseSummaryVm>> GetActiveCaseSummariesByOwner(string ownerId)
	{
		var query = _concernsDbContext.ConcernsCase
			.Include(cases => cases.Rating)
			.Include(cases => cases.Status)
			.Include(cases => cases.Decisions).ThenInclude(d => d.DecisionTypes)
			.Where(cases => cases.CreatedBy == ownerId && cases.Status.Name == "Live")
			.Select (cases => new ActiveCaseSummaryVm
			{	
				CaseUrn = cases.Urn,
				CreatedAt = cases.CreatedAt,
				CreatedBy = cases.CreatedBy,
				Rating = cases.Rating,
				StatusName = cases.Status.Name,
				TrustUkPrn = cases.TrustUkprn,
				UpdatedAt = cases.UpdatedAt,
				
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
					.Select(action => new CaseSummaryVm.Action(action.CreatedAt.Date, null, "Action: Trust Financial Forecast (TFF)"))
					.ToArray()
			})
			.AsSplitQuery();

		return await query.ToListAsync();
	}
	
	public async Task<IList<ClosedCaseSummaryVm>> GetClosedCaseSummariesByOwner(string ownerId)
	{
		var query = _concernsDbContext.ConcernsCase
			.Include(cases => cases.Rating)
			.Include(cases => cases.Decisions).ThenInclude(d => d.DecisionTypes)
			.Where(cases => cases.CreatedBy == ownerId && cases.Status.Name == "Close")
			.Select (cases => new ClosedCaseSummaryVm
			{	
				CaseUrn = cases.Urn,
				ClosedAt = cases.ClosedAt.Value,
				CreatedAt = cases.CreatedAt,
				CreatedBy = cases.CreatedBy,
				StatusName = cases.Status.Name,
				TrustUkPrn = cases.TrustUkprn,
				UpdatedAt = cases.UpdatedAt,
					
				ClosedConcerns = from concerns in cases.ConcernsRecords where concerns.StatusId == 3 select new CaseSummaryVm.Concern(concerns.ConcernsType.ToString(), concerns.ConcernsRating, concerns.CreatedAt),
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
					.Select(action => new CaseSummaryVm.Action(action.CreatedAt.Date, action.ClosedAt.Value.DateTime, "Action: Trust Financial Forecast (TFF)"))
					.ToArray()
			})
			.AsSplitQuery();

		return await query.ToListAsync();
	}
		
	public async Task<IList<ClosedCaseSummaryVm>> GetClosedCaseSummariesByTrust(string trustUkPrn)
	{
		var query = _concernsDbContext.ConcernsCase
			.Include(cases => cases.Rating)
			.Include(cases => cases.Status)
			.Include(cases => cases.Decisions).ThenInclude(d => d.DecisionTypes)
			.Where(cases => cases.TrustUkprn == trustUkPrn && cases.Status.Name == "Close")
			.Select (cases => new ClosedCaseSummaryVm
			{	
				CaseUrn = cases.Urn,
				ClosedAt = cases.ClosedAt.Value,
				CreatedAt = cases.CreatedAt,
				CreatedBy = cases.CreatedBy,
				StatusName = cases.Status.Name,
				TrustUkPrn = cases.TrustUkprn,
				UpdatedAt = cases.UpdatedAt,
					
				ClosedConcerns = from concerns in cases.ConcernsRecords where concerns.StatusId == 3 select new CaseSummaryVm.Concern(concerns.ConcernsType.ToString(), concerns.ConcernsRating, concerns.CreatedAt),
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
					.Select(action => new CaseSummaryVm.Action(action.CreatedAt.Date, action.ClosedAt.Value.DateTime, "Action: Trust Financial Forecast (TFF)"))
					.ToArray()
			})
			.AsSplitQuery();

		return await query.ToListAsync();
	}
		public async Task<IList<ActiveCaseSummaryVm>> GetActiveCaseSummariesByTrust(string trustUkPrn)
	{
		var query = _concernsDbContext.ConcernsCase
			.Include(cases => cases.Rating)
			.Include(cases => cases.Status)
			.Include(cases => cases.Decisions).ThenInclude(d => d.DecisionTypes)
			.Where(cases => cases.TrustUkprn == trustUkPrn && cases.Status.Name == "Live")
			.Select (cases => new ActiveCaseSummaryVm
			{	
				CaseUrn = cases.Urn,
				CreatedAt = cases.CreatedAt,
				CreatedBy = cases.CreatedBy,
				Rating = cases.Rating,
				StatusName = cases.Status.Name,
				TrustUkPrn = cases.TrustUkprn,
				UpdatedAt = cases.UpdatedAt,
					
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
					.Select(action => new CaseSummaryVm.Action(action.CreatedAt.Date, null, "Action: Trust Financial Forecast (TFF)"))
					.ToArray()
			})
			.AsSplitQuery();

		return await query.ToListAsync();
	}
}

