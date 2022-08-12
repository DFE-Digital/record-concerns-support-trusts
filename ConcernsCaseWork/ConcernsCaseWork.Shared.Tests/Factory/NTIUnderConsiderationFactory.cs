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

		public static List<NtiUnderConsiderationModel> BuildListNTIUnderConsiderationModel(DateTime? closedAt = null)
		{
			return new List<NtiUnderConsiderationModel>
			{
				BuildNTIUnderConsiderationModel(closedAt)
			};
		}

		public static NtiUnderConsiderationModel BuildNTIUnderConsiderationModel(DateTime? closedAt = null)
		{
			var ntiUnderConsideration = new NtiUnderConsiderationModel();
			ntiUnderConsideration.Id = Fixture.Create<long>();
			ntiUnderConsideration.CaseUrn = Fixture.Create<long>();
			ntiUnderConsideration.Notes = Fixture.Create<string>();
			ntiUnderConsideration.ClosedAt = closedAt;

			return ntiUnderConsideration;
		}
	}
}