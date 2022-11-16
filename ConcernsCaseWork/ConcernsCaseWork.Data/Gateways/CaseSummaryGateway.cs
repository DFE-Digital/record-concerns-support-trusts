using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
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
				Decisions = from decisions in cases.Decisions select decisions,
				FinancialPlanCases = _concernsDbContext.FinancialPlanCases.Where(x => x.CaseUrn == cases.Urn).Select(action => new CaseSummaryVm.Action(action.CreatedAt, action.ClosedAt, "Action: Financial plan")).ToArray(),
				NtisUnderConsideration = _concernsDbContext.NTIUnderConsiderations.Where(x => x.CaseUrn == cases.Urn).Select(action => new CaseSummaryVm.Action(action.CreatedAt, action.ClosedAt, "Action: NTI under consideration")).ToArray(),
            	NtiWarningLetters = _concernsDbContext.NTIWarningLetters.Where(x => x.CaseUrn == cases.Urn).Select(action => new CaseSummaryVm.Action(action.CreatedAt, action.ClosedAt, "Action: NTI warning letter")).ToArray(),
            	NoticesToImprove = _concernsDbContext.NoticesToImprove.Where(x => x.CaseUrn == cases.Urn).Select(action => new CaseSummaryVm.Action(action.CreatedAt, action.ClosedAt, "Action: Notice To Improve")).ToArray(),
				SrmaCases = _concernsDbContext.SRMACases.Where(x => x.CaseUrn == cases.Urn).Select(action => new CaseSummaryVm.Action(action.CreatedAt, action.ClosedAt, "Action: School Resource Management Adviser")).ToArray()
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
	public IEnumerable<Decision> Decisions { get; set; }
	public IEnumerable<Action> FinancialPlanCases { get; set; }
	public IEnumerable<Action> NoticesToImprove { get; set; }
	public IEnumerable<Action> NtiWarningLetters { get; set; }
	public IEnumerable<Action> NtisUnderConsideration { get; set; }
	public IEnumerable<Action> SrmaCases { get; set; }
	
	public record Action(DateTime CreatedAt, DateTime? ClosedAt, string Name);
	public record Concern(string Name, ConcernsRating Rating, DateTime CreatedAt);
}