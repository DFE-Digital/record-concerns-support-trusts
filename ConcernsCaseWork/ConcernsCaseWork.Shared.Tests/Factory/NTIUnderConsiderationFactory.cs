using AutoFixture;
using ConcernsCaseWork.Models.CaseActions;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class NTIUnderConsiderationFactory
	{
		private readonly static Fixture Fixture = new Fixture();

		public static List<NtiUnderConsiderationModel> BuildListNTIUnderConsiderationModel()
		{
			return new List<NtiUnderConsiderationModel>
			{
				BuildNTIUnderConsiderationModel(),
				BuildClosedNTIUnderConsiderationModel()
			};
		}
		
		public static List<NtiUnderConsiderationModel> BuildClosedListNTIUnderConsiderationModel()
		{
			return new List<NtiUnderConsiderationModel>
			{
				BuildClosedNTIUnderConsiderationModel(),
				BuildClosedNTIUnderConsiderationModel()
			};
		}

		public static NtiUnderConsiderationModel BuildNTIUnderConsiderationModel()
		{
			var ntiUnderConsideration = new NtiUnderConsiderationModel();
			ntiUnderConsideration.Id = Fixture.Create<long>();
			ntiUnderConsideration.CaseUrn = Fixture.Create<long>();
			ntiUnderConsideration.Notes = Fixture.Create<string>();

			return ntiUnderConsideration;
		}
		public static NtiUnderConsiderationModel BuildClosedNTIUnderConsiderationModel()
		{
			var ntiUnderConsideration = BuildNTIUnderConsiderationModel();
			
			ntiUnderConsideration.ClosedAt = Fixture.Create<DateTime>();
			ntiUnderConsideration.ClosedStatusId = Fixture.Create<int>();
			ntiUnderConsideration.ClosedStatusName = Fixture.Create<string>();

			return ntiUnderConsideration;
		}
	}
}