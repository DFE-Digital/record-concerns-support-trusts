using AutoFixture;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models.CaseActions;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class SrmaFactory
	{
		private readonly static Fixture Fixture = new Fixture();

		public static List<SRMAModel> BuildListSrmaModel(SRMAStatus status = SRMAStatus.Deployed, SRMAReasonOffered reason = SRMAReasonOffered.Unknown, DateTime? closedAt = null)
		{
			return new List<SRMAModel>
			{
				BuildSrmaModel(status, reason, closedAt)
			};
		}

		public static SRMAModel BuildSrmaModel(SRMAStatus status, SRMAReasonOffered reason = SRMAReasonOffered.Unknown, DateTime? closedAt = null)
		{

			var srma = new SRMAModel
			{
				Id = Fixture.Create<long>(),
				CaseUrn = Fixture.Create<long>(),
				DateOffered = Fixture.Create<DateTime>(),
				DateAccepted = Fixture.Create<DateTime>(),
				DateReportSentToTrust = Fixture.Create<DateTime>(),
				DateVisitStart = Fixture.Create<DateTime>(),
				DateVisitEnd = Fixture.Create<DateTime>(),
				Status = status,
				Notes = Fixture.Create<string>(),
				Reason = reason,
				CreatedAt = Fixture.Create<DateTime>(),
				ClosedAt = closedAt
			};

			return srma;
		}
	}
}