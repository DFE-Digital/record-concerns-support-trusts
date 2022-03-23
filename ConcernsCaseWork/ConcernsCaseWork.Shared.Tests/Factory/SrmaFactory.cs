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

		public static List<SRMA> BuildListSrmaModel(SRMAStatus status)
		{
			return new List<SRMA>
			{
				BuildSrmaModel(status)
			};
		}

		public static SRMA BuildSrmaModel(SRMAStatus status)
		{
			return new SRMA
			{
				DateOffered = Fixture.Create<DateTime>(),
				Status = status,
				Notes = Fixture.Create<string>()
			};
		}
	}
}