using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Features.ConcernsRating;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.API.Extensions;
using ConcernsCaseWork.Utils.Extensions;

namespace ConcernsCaseWork.API.Features.Case;

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
			TrustFinancialForecasts = Create(caseSummary.TrustFinancialForecasts),
			StatusName = caseSummary.StatusName,
			TrustUkPrn = caseSummary.TrustUkPrn,
			UpdatedAt = caseSummary.UpdatedAt,
			CaseLastUpdatedAt = caseSummary.CaseLastUpdatedAt,
			Division = caseSummary.Division,
			Area = getArea(caseSummary)
		};
	}

	public static ClosedCaseSummaryResponse Create(ClosedCaseSummaryVm caseSummary)
	{
		return new ClosedCaseSummaryResponse
		{
			ClosedConcerns = Create(caseSummary.ClosedConcerns),
			CaseUrn = caseSummary.CaseUrn,
			ClosedAt = caseSummary.ClosedAt,
			CreatedAt = caseSummary.CreatedAt,
			CreatedBy = caseSummary.CreatedBy,
			Decisions = Create(caseSummary.Decisions),
			FinancialPlanCases = Create(caseSummary.FinancialPlanCases),
			NtiWarningLetters = Create(caseSummary.NtiWarningLetters),
			NtisUnderConsideration = Create(caseSummary.NtisUnderConsideration),
			NoticesToImprove = Create(caseSummary.NoticesToImprove),
			SrmaCases = Create(caseSummary.SrmaCases),
			TrustFinancialForecasts = Create(caseSummary.TrustFinancialForecasts),
			StatusName = caseSummary.StatusName,
			TrustUkPrn = caseSummary.TrustUkPrn,
			UpdatedAt = caseSummary.UpdatedAt,
			Division = caseSummary.Division,
			Area = getArea(caseSummary)
		};
	}

	private static string getArea(CaseSummaryVm caseSummary)
	{
		return caseSummary.Division == Division.RegionsGroup ? caseSummary.Region?.Description() : caseSummary.Territory?.Description();
	}

	private static CaseSummaryResponse.Concern Create(CaseSummaryVm.Concern concern)
		=> new(concern.Name, ConcernsRatingResponseFactory.Create(concern.Rating), concern.CreatedAt);

	private static IEnumerable<CaseSummaryResponse.Concern> Create(IEnumerable<CaseSummaryVm.Concern> concerns)
		=> concerns == null ? Array.Empty<CaseSummaryResponse.Concern>() : concerns.Select(Create);

	private static CaseSummaryResponse.ActionOrDecision Create(CaseSummaryVm.Action action)
		=> new(action.CreatedAt, action.ClosedAt, action.Name);

	private static IEnumerable<CaseSummaryResponse.ActionOrDecision> Create(IEnumerable<CaseSummaryVm.Action> actions)
		=> actions == null ? Array.Empty<CaseSummaryResponse.ActionOrDecision>() : actions.Select(Create);

	private static CaseSummaryResponse.ActionOrDecision Create(Data.Models.Decisions.Decision decision)
		=> new(decision.CreatedAt.DateTime, decision.ClosedAt?.DateTime, "Decision: " + decision.GetTitle());

	private static IEnumerable<CaseSummaryResponse.ActionOrDecision> Create(IEnumerable<Data.Models.Decisions.Decision> decisions)
		=> decisions == null ? Array.Empty<CaseSummaryResponse.ActionOrDecision>() : decisions.Select(Create);
}