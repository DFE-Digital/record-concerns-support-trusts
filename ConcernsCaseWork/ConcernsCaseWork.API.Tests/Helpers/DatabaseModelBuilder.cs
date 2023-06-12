using AutoFixture;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.Tests.Helpers
{
	public class DatabaseModelBuilder
	{
		private static Fixture _fixture = new Fixture();

		public static FinancialPlanCase BuildFinancialPlan(int caseId)
		{
			return new FinancialPlanCase() { CaseUrn = caseId, CreatedAt = _fixture.Create<DateTime>(), UpdatedAt = _fixture.Create<DateTime>() };
		}

		public static Decision BuildDecision(int caseId)
		{
			return new Decision() { ConcernsCaseId = caseId, CreatedAt = _fixture.Create<DateTime>(), UpdatedAt = _fixture.Create<DateTime>() };
		}

		public static NoticeToImprove BuildNoticeToImprove(int caseId)
		{
			return new NoticeToImprove() { CaseUrn = caseId, CreatedAt = _fixture.Create<DateTime>(), UpdatedAt = _fixture.Create<DateTime>() };
		}

		public static NTIUnderConsideration BuildNTIUnderConsideration(int caseId)
		{
			return new NTIUnderConsideration() { CaseUrn = caseId, CreatedAt = _fixture.Create<DateTime>(), UpdatedAt = _fixture.Create<DateTime>() };
		}

		public static NTIWarningLetter BuildNTIWarningLetter(int caseId)
		{
			return new NTIWarningLetter() { CaseUrn = caseId, CreatedAt = _fixture.Create<DateTime>(), UpdatedAt = _fixture.Create<DateTime>() };
		}

		public static SRMACase BuildSrma(int caseId)
		{
			return new SRMACase() { CaseUrn = caseId, CreatedAt = _fixture.Create<DateTime>(), UpdatedAt = _fixture.Create<DateTime>() };
		}

		public static TrustFinancialForecast BuildTrustFinancialForecast(int caseId)
		{
			return new TrustFinancialForecast() { CaseUrn = caseId, CreatedAt = _fixture.Create<DateTime>(), UpdatedAt = _fixture.Create<DateTime>() };
		}
	}
}
