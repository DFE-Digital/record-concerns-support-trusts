using AutoFixture;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models.CaseActions;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class NTIUnderConsiderationFactory
	{
		private readonly static Fixture Fixture = new Fixture();

		public static List<NtiModel> BuildListNTIUnderConsiderationModel()
		{
			return new List<NtiModel>
			{
				BuildNTIUnderConsiderationModel()
			};
		}

		public static NtiModel BuildNTIUnderConsiderationModel()
		{
			var ntiUnderConsideration = new NtiModel();
			ntiUnderConsideration.Id = Fixture.Create<long>();
			ntiUnderConsideration.CaseUrn = Fixture.Create<long>();
			ntiUnderConsideration.Notes = Fixture.Create<string>();

			return ntiUnderConsideration;
		}
	}
}