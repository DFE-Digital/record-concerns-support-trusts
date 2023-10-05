using AutoFixture;
using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Models.Decisions;
using MoreLinq;
using System;
using System.Collections.Generic;

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

		public static ConcernsCase BuildCase()
		{
			var result = new ConcernsCase()
			{
				RatingId = (int)ConcernRating.RedPlus,
				StatusId = (int)CaseStatus.Live,
				TrustUkprn = CreateUkPrn(),
				ConcernsRecords = new List<ConcernsRecord>(),
				CreatedAt = _fixture.Create<DateTime>(),
				CreatedBy = _fixture.Create<string>()
			};

			return result;
		}

		public static ConcernsCase CloseCase(ConcernsCase concernsCase)
		{
			concernsCase.ClosedAt = _fixture.Create<DateTime>();
			concernsCase.StatusId = (int)CaseStatus.Close;
			concernsCase.ConcernsRecords.ForEach(r => r.StatusId = (int)CaseStatus.Close);

			return concernsCase;
		}

		public static ConcernsRecord BuildConcernsRecord()
		{
			return new ConcernsRecord()
			{
				RatingId = (int)ConcernRating.AmberGreen,
				StatusId = (int)CaseStatus.Live,
				TypeId = (int)ConcernType.FinancialDeficit,
				CreatedAt = _fixture.Create<DateTime>()
			};
		}

		public static string CreateUkPrn()
		{
			return _fixture.Create<string>().Substring(0, 7);
		}
	}
}
