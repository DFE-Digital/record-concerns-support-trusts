using ConcernsCaseWork.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data.Gateways;

public class CaseSummaryGateway : ICaseSummaryGateway
{
	private readonly ConcernsDbContext _concernsDbContext;

	public CaseSummaryGateway(ConcernsDbContext concernsDbContext)
	{
		_concernsDbContext = concernsDbContext;
	}

	public async Task<IList<CaseSummaryVm>> GetActiveCaseSummaries(string ownerId)
	{
		var query = _concernsDbContext.ConcernsCase
			.Include(cases => cases.Rating)
			.Include(cases => cases.Status)
			.Where(cases => cases.CreatedBy == ownerId && cases.Status.Name == "Live")
			.Select (cases => new CaseSummaryVm
			{	
				CaseUrn = cases.Urn,
				CreatedAt = cases.CreatedAt,
				CreatedBy = cases.CreatedBy,
				Rating = cases.Rating,
				StatusName = cases.Status.Name,
				TrustUkPrn = cases.TrustUkprn,
				UpdatedAt = cases.UpdatedAt,
				
				ActiveConcerns = from concerns in cases.ConcernsRecords where concerns.StatusId == 1 select new CaseSummaryVm.Concern(concerns.ConcernsType.ToString(), concerns.ConcernsRating, concerns.CreatedAt),
				Decisions = from decisions in cases.Decisions select new CaseSummaryVm.ActionOrDecision(decisions.CreatedAt.DateTime, decisions.ClosedAt.HasValue ? decisions.ClosedAt.Value.DateTime : null, "Decision"),
				FinancialPlanCases = _concernsDbContext.FinancialPlanCases.Where(x => x.CaseUrn == cases.Urn).Select(action => new CaseSummaryVm.ActionOrDecision(action.CreatedAt, action.ClosedAt, "Action: Financial plan")).ToList(),
				NtisUnderConsideration = _concernsDbContext.NTIUnderConsiderations.Where(x => x.CaseUrn == cases.Urn).Select(action => new CaseSummaryVm.ActionOrDecision(action.CreatedAt, action.ClosedAt, "Action: NTI under consideration")).ToList(),
            	NtiWarningLetters = _concernsDbContext.NTIWarningLetters.Where(x => x.CaseUrn == cases.Urn).Select(action => new CaseSummaryVm.ActionOrDecision(action.CreatedAt, action.ClosedAt, "Action: NTI warning letter")).ToList(),
            	NoticesToImprove = _concernsDbContext.NoticesToImprove.Where(x => x.CaseUrn == cases.Urn).Select(action => new CaseSummaryVm.ActionOrDecision(action.CreatedAt, action.ClosedAt, "Action: Notice To Improve")).ToList(),
				SrmaCases = _concernsDbContext.SRMACases.Where(x => x.CaseUrn == cases.Urn).Select(action => new CaseSummaryVm.ActionOrDecision(action.CreatedAt, action.ClosedAt, "Action: School Resource Management Adviser")).ToList()
			});

		return await query.ToListAsync();
	}
}



public record CaseSummaryVm
{
	public long CaseUrn { get; set; }
	public string CreatedBy { get; set; }
	public DateTime CreatedAt { get; set; }
	public ConcernsRating Rating { get; set; }
	public string StatusName { get; set; }
	public string TrustUkPrn { get; set; }
	public DateTime UpdatedAt { get; set; }
	public IEnumerable<Concern> ActiveConcerns { get; set; }
	public IEnumerable<ActionOrDecision> Decisions { get; set; }
	public IEnumerable<ActionOrDecision> FinancialPlanCases { get; set; }
	public IEnumerable<ActionOrDecision> NoticesToImprove { get; set; }
	public IEnumerable<ActionOrDecision> NtiWarningLetters { get; set; }
	public IEnumerable<ActionOrDecision> NtisUnderConsideration { get; set; }
	public IEnumerable<ActionOrDecision> SrmaCases { get; set; }
	
	public record ActionOrDecision(DateTime CreatedAt, DateTime? ClosedAt, string Name);
	public record Concern(string Name, ConcernsRating Rating, DateTime CreatedAt);
}