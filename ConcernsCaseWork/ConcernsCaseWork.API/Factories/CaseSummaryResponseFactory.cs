using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Factories;

public static class CaseSummaryResponseFactory
{
	public static ActiveCaseSummaryResponse Create(CaseSummary caseSummary)
	{
		return new ActiveCaseSummaryResponse
		{
			ActiveConcerns = caseSummary.ActiveConcerns,
			CaseUrn = caseSummary.CaseUrn,
			CreatedAt = caseSummary.CreatedAt,
			CreatedBy = caseSummary.CreatedBy,
			Decisions = caseSummary.Decisions,
			FinancialPlanCases = caseSummary.FinancialPlanCases,
			NtiWarningLetters = caseSummary.NtiWarningLetters,
			NtisUnderConsideration = caseSummary.NtisUnderConsideration,
			NoticesToImprove = caseSummary.NoticesToImprove,
			Rating = ConcernsRatingResponseFactory.Create(caseSummary.Rating),
			SrmaCases = caseSummary.SrmaCases,
			StatusName = caseSummary.StatusName,
			TrustUkPrn = caseSummary.TrustUkPrn,
			UpdatedAt = caseSummary.UpdatedAt
		};
	}
}