using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Models;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.API.Tests.Helpers
{
	public class CaseSummaryAssert
	{
		public static void AssertCaseActions(CaseSummaryResponse actualCase)
		{
			actualCase.Decisions.Should().HaveCount(1);
			var decision = actualCase.Decisions.First();
			decision.Name.Should().Be("Decision: No Decision Types");

			actualCase.NoticesToImprove.Should().HaveCount(1);
			var nti = actualCase.NoticesToImprove.First();
			nti.Name.Should().Be("Action: Notice To Improve");

			actualCase.NtisUnderConsideration.Should().HaveCount(1);
			var ntiUnderConsideration = actualCase.NtisUnderConsideration.First();
			ntiUnderConsideration.Name.Should().Be("Action: NTI under consideration");

			actualCase.NtiWarningLetters.Should().HaveCount(1);
			var ntiWarningLetter = actualCase.NtiWarningLetters.First();
			ntiWarningLetter.Name.Should().Be("Action: NTI warning letter");

			actualCase.FinancialPlanCases.Should().HaveCount(1);
			var financialPlan = actualCase.FinancialPlanCases.First();
			financialPlan.Name.Should().Be("Action: Financial plan");

			actualCase.SrmaCases.Should().HaveCount(1);
			var srma = actualCase.SrmaCases.First();
			srma.Name.Should().Be("Action: School Resource Management Adviser");

			actualCase.TrustFinancialForecasts.Should().HaveCount(1);
			var tff = actualCase.TrustFinancialForecasts.First();
			tff.Name.Should().Be("Action: Trust Financial Forecast (TFF)");
		}

		public static void AssertCaseList(List<CaseSummaryResponse> actualCases, List<ConcernsCase> expectedCases)
		{
			for (var idx = 0; idx < expectedCases.Count; idx++)
			{
				var expectedCase = expectedCases[idx];
				var actualCase = actualCases[idx];

				actualCase.TrustUkPrn.Should().Be(expectedCase.TrustUkprn);
				actualCase.CaseUrn.Should().Be(expectedCase.Id);
				actualCase.CreatedBy.Should().Be(expectedCase.CreatedBy);
				actualCase.CaseLastUpdatedAt.Should().Be(expectedCase.CaseLastUpdatedAt);
			}
		}
	}
}
