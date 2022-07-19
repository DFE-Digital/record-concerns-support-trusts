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

		public static List<NtiUnderConsiderationModel> BuildListNTIUnderConsiderationModel()
		{
			return new List<NtiUnderConsiderationModel>
			{
				BuildNTIUnderConsiderationModel()
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
	}
}