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

	public async Task<IList<CaseSummary>> GetActiveCaseSummaries(string ownerId)
	{
		var query = _concernsDbContext.ConcernsCase
			.Include(cases => cases.Rating)
			.Include(cases => cases.Status)
			.Where(cases => cases.CreatedBy == ownerId && cases.Status.Name == "Live")
			.Select (cases => new CaseSummary
			{
				SrmaCases = _concernsDbContext.SRMACases.Where(x => x.CaseUrn == cases.Urn).Select(action => new Summary(action.CreatedAt, action.ClosedAt, "Action: School Resource Management Adviser")).ToList(),
				FinancialPlanCases = _concernsDbContext.FinancialPlanCases.Where(x => x.CaseUrn == cases.Urn).Select(action => new Summary(action.CreatedAt, action.ClosedAt, "Action: Financial plan")).ToList(),
				NtisUnderConsideration = _concernsDbContext.NTIUnderConsiderations.Where(x => x.CaseUrn == cases.Urn).Select(action => new Summary(action.CreatedAt, action.ClosedAt, "Action: NTI under consideration")).ToList(),
				NtiWarningLetters = _concernsDbContext.NTIWarningLetters.Where(x => x.CaseUrn == cases.Urn).Select(action => new Summary(action.CreatedAt, action.ClosedAt, "Action: NTI warning letter")).ToList(),
				NoticesToImprove = _concernsDbContext.NoticesToImprove.Where(x => x.CaseUrn == cases.Urn).Select(action => new Summary(action.CreatedAt, action.ClosedAt, "Action: Notice To Improve")).ToList(),
				ActiveConcerns = from concerns in cases.ConcernsRecords where concerns.StatusId == 1 select concerns.ConcernsType.ToString(),
				CaseUrn = cases.Urn,
				CreatedAt = cases.CreatedAt,
				CreatedBy = cases.CreatedBy,
				Rating = cases.Rating,
				StatusName = cases.Status.Name,
				TrustUkPrn = cases.TrustUkprn,
				UpdatedAt = cases.UpdatedAt
			});

		return await query.ToListAsync();
	}
}

public record Summary(DateTime CreatedAt, DateTime? ClosedAt, string Name);
public record CaseSummary
{
	public long CaseUrn { get; set; }
	public string CreatedBy { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	public string StatusName { get; set; }
	public ConcernsRating Rating { get; set; }
	public string TrustUkPrn { get; set; }
	public IEnumerable<string> ActiveConcerns { get; set; }
	public IEnumerable<Summary> FinancialPlanCases { get; set; }
	public IEnumerable<Summary> NoticesToImprove { get; set; }
	public IEnumerable<Summary> NtiWarningLetters { get; set; }
	public IEnumerable<Summary> NtisUnderConsideration { get; set; }
	public IEnumerable<Summary> SrmaCases { get; set; }
}