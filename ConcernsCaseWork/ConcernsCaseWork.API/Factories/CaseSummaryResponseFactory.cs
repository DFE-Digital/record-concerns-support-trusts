using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Factories;

public static class CaseSummaryResponseFactory
{
	public static ActiveCaseSummaryResponse Create(CaseSummary caseSummary)
	{
		return new ActiveCaseSummaryResponse
		{
			CaseUrn = caseSummary.CaseUrn,
			CreatedAt = caseSummary.CreatedAt,
			UpdatedAt = caseSummary.UpdatedAt,
			StatusName = caseSummary.StatusName,
			Rating = ConcernsRatingResponseFactory.Create(caseSummary.Rating),
			TrustUkPrn = caseSummary.TrustUkPrn,
			SrmaCases = caseSummary.SrmaCases,
			FinancialPlanCases = caseSummary.FinancialPlanCases,
			NtiWarningLetters = caseSummary.NtiWarningLetters,
			NtisUnderConsideration = caseSummary.NtisUnderConsideration,
			NoticesToImprove = caseSummary.NoticesToImprove,
			ActiveConcerns = caseSummary.ActiveConcerns,
			CreatedBy = caseSummary.CreatedBy
		};
	}
}