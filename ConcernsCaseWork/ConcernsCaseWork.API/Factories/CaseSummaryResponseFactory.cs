using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Factories;

public static class CaseSummaryResponseFactory
{
	public static ActiveCaseSummaryResponse Create(CaseSummaryVm caseSummary)
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
	
	private static ActiveCaseSummaryResponse.Concern Create(CaseSummaryVm.Concern concern)
		=> new (concern.Name, ConcernsRatingResponseFactory.Create(concern.Rating), concern.CreatedAt);
	
	private static IEnumerable<ActiveCaseSummaryResponse.Concern> Create(IEnumerable<CaseSummaryVm.Concern> concerns)
		=> concerns.Select(Create);

	private static ActiveCaseSummaryResponse.ActionOrDecision Create(CaseSummaryVm.ActionOrDecision actionOrDecision)
		=> new (actionOrDecision.CreatedAt, actionOrDecision.ClosedAt, actionOrDecision.Name);
	
	private static IEnumerable<ActiveCaseSummaryResponse.ActionOrDecision> Create(IEnumerable<CaseSummaryVm.ActionOrDecision> actionsOrDecisions)
		=> actionsOrDecisions.Select(Create);
	
}