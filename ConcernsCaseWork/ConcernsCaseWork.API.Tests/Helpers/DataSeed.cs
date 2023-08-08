using AutoFixture;
using ConcernsCaseWork.Data;
using ConcernsCaseWork.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.Tests.Helpers
{
	public static class DataSeed
	{
		private static Fixture _fixture = new();

		public static async Task<List<ConcernsCase>> SaveCases(this ConcernsDbContext context, List<ConcernsCase> cases)
		{
			context.ConcernsCase.AddRange(cases);
			await context.SaveChangesAsync();

			return cases;
		}

		public static async Task CreateOpenCaseActions(this ConcernsDbContext context, int caseId)
		{
			context.Decisions.Add(DatabaseModelBuilder.BuildDecision(caseId));
			context.NoticesToImprove.Add(DatabaseModelBuilder.BuildNoticeToImprove(caseId));
			context.NTIUnderConsiderations.Add(DatabaseModelBuilder.BuildNTIUnderConsideration(caseId));
			context.NTIWarningLetters.Add(DatabaseModelBuilder.BuildNTIWarningLetter(caseId));
			context.SRMACases.Add(DatabaseModelBuilder.BuildSrma(caseId));
			context.TrustFinancialForecasts.Add(DatabaseModelBuilder.BuildTrustFinancialForecast(caseId));
			context.FinancialPlanCases.Add(DatabaseModelBuilder.BuildFinancialPlan(caseId));

			await context.SaveChangesAsync();
		}

		public static  async Task CreateClosedCaseActions(this ConcernsDbContext context, int caseId)
		{
			var decision = DatabaseModelBuilder.BuildDecision(caseId);
			decision.ClosedAt = _fixture.Create<DateTime>();
			context.Decisions.Add(decision);

			var noticeToImprove = DatabaseModelBuilder.BuildNoticeToImprove(caseId);
			noticeToImprove.ClosedAt = _fixture.Create<DateTime>();
			context.NoticesToImprove.Add(noticeToImprove);

			var ntiUnderConsideration = DatabaseModelBuilder.BuildNTIUnderConsideration(caseId);
			ntiUnderConsideration.ClosedAt = _fixture.Create<DateTime>();
			context.NTIUnderConsiderations.Add(ntiUnderConsideration);

			var ntiWarningLetter = DatabaseModelBuilder.BuildNTIWarningLetter(caseId);
			ntiWarningLetter.ClosedAt = _fixture.Create<DateTime>();
			context.NTIWarningLetters.Add(ntiWarningLetter);

			var srma = DatabaseModelBuilder.BuildSrma(caseId);
			srma.ClosedAt = _fixture.Create<DateTime>();
			context.SRMACases.Add(srma);

			var tff = DatabaseModelBuilder.BuildTrustFinancialForecast(caseId);
			tff.ClosedAt = _fixture.Create<DateTime>();
			context.TrustFinancialForecasts.Add(tff);

			var financialPlan = DatabaseModelBuilder.BuildFinancialPlan(caseId);
			financialPlan.ClosedAt = _fixture.Create<DateTime>();
			context.FinancialPlanCases.Add(financialPlan);

			await context.SaveChangesAsync();
		}
	}
}
