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

		public static List<SRMAModel> BuildListSrmaModel(SRMAStatus status)
		{
			return new List<SRMAModel>
			{
				BuildSrmaModel(status)
			};
		}

		public static SRMAModel BuildSrmaModel(SRMAStatus status)
		{
			return new SRMAModel
			{
				Id = Fixture.Create<long>(),
				CaseUrn = Fixture.Create<long>(),
				DateOffered = Fixture.Create<DateTime>(),
				Status = status,
				Notes = Fixture.Create<string>()
			};
		}
	}
}