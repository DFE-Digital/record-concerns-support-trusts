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

		public static List<SRMAModel> BuildListSrmaModel(SRMAStatus status = SRMAStatus.Deployed, SRMAReasonOffered reason = SRMAReasonOffered.Unknown)
		{
			return new List<SRMAModel>
			{
				BuildSrmaModel(status, reason)
			};
		}

		public static SRMAModel BuildSrmaModel(SRMAStatus status, SRMAReasonOffered reason = SRMAReasonOffered.Unknown)
		{
			var srma = new SRMAModel
			(
				Fixture.Create<long>(),
				Fixture.Create<long>(),
				Fixture.Create<DateTime>(),
				Fixture.Create<DateTime>(),
				Fixture.Create<DateTime>(),
				Fixture.Create<DateTime>(),
				Fixture.Create<DateTime>(),
				status,
				Fixture.Create<string>(),
				reason,
				Fixture.Create<DateTime>()
			);

			return srma;
		}
	}
}