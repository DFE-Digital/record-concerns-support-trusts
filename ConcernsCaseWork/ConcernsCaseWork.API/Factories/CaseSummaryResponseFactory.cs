using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;

namespace ConcernsCaseWork.API.Factories;

public static class CaseSummaryResponseFactory
{
	public static ActiveCaseSummaryResponse Create(ActiveCaseSummaryVm caseSummary)
	{
		return new ActiveCaseSummaryResponse
		{
			ActiveConcerns = Create(caseSummary.ActiveConcerns),
			CaseUrn = caseSummary.CaseUrn,
			CreatedAt = caseSummary.CreatedAt,
			CreatedBy = caseSummary.CreatedBy,
			Decisions = Create(caseSummary.Decisions),
			FinancialPlanCases = Create(caseSummary.FinancialPlanCases),
			NtiWarningLetters = Create(caseSummary.NtiWarningLetters),
			NtisUnderConsideration = Create(caseSummary.NtisUnderConsideration),
			NoticesToImprove = Create(caseSummary.NoticesToImprove),
			Rating = ConcernsRatingResponseFactory.Create(caseSummary.Rating),
			SrmaCases = Create(caseSummary.SrmaCases),
			StatusName = caseSummary.StatusName,
			TrustUkPrn = caseSummary.TrustUkPrn,
			UpdatedAt = caseSummary.UpdatedAt
		};
	}
	public static ClosedCaseSummaryResponse Create(ClosedCaseSummaryVm caseSummary)
	{
		return new ClosedCaseSummaryResponse
		{
			ClosedConcerns = Create(caseSummary.ClosedConcerns),
			CaseUrn = caseSummary.CaseUrn,
			CreatedAt = caseSummary.CreatedAt,
			CreatedBy = caseSummary.CreatedBy,
			Decisions = Create(caseSummary.Decisions),
			FinancialPlanCases = Create(caseSummary.FinancialPlanCases),
			NtiWarningLetters = Create(caseSummary.NtiWarningLetters),
			NtisUnderConsideration = Create(caseSummary.NtisUnderConsideration),
			NoticesToImprove = Create(caseSummary.NoticesToImprove),
			Rating = ConcernsRatingResponseFactory.Create(caseSummary.Rating),
			SrmaCases = Create(caseSummary.SrmaCases),
			StatusName = caseSummary.StatusName,
			TrustUkPrn = caseSummary.TrustUkPrn,
			UpdatedAt = caseSummary.UpdatedAt
		};
	}
	
	private static CaseSummaryResponse.Concern Create(CaseSummaryVm.Concern concern)
		=> new (concern.Name, ConcernsRatingResponseFactory.Create(concern.Rating), concern.CreatedAt);
	
	private static IEnumerable<CaseSummaryResponse.Concern> Create(IEnumerable<CaseSummaryVm.Concern> concerns)
		=> concerns == null ? Array.Empty<CaseSummaryResponse.Concern>() : concerns.Select(Create);

	private static CaseSummaryResponse.ActionOrDecision Create(CaseSummaryVm.Action action)
		=> new (action.CreatedAt, action.ClosedAt, action.Name);
	
	private static IEnumerable<CaseSummaryResponse.ActionOrDecision> Create(IEnumerable<CaseSummaryVm.Action> actions)
		=> actions == null ? Array.Empty<CaseSummaryResponse.ActionOrDecision>() : actions.Select(Create);
	
	private static CaseSummaryResponse.ActionOrDecision Create(Decision decision)
		=> new (decision.CreatedAt.DateTime, decision.ClosedAt?.DateTime, "Decision: " + decision.GetTitle());
	
	private static IEnumerable<CaseSummaryResponse.ActionOrDecision> Create(IEnumerable<Decision> decisions)
		=> decisions == null ? Array.Empty<CaseSummaryResponse.ActionOrDecision>() : decisions.Select(Create);
}