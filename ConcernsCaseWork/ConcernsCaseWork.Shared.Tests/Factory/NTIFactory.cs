using AutoFixture;
using ConcernsCaseWork.Models.CaseActions;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class NTIFactory
	{
		private readonly static Fixture Fixture = new Fixture();

		public static List<NtiModel> BuildListNTIModel()
		{
			return new List<NtiModel>
			{
				BuildNTIModel(),
				BuildClosedNTIModel()
			};
		}
		
		public static List<NtiModel> BuildClosedListNTIModel()
		{
			return new List<NtiModel>
			{
				BuildClosedNTIModel(),
				BuildClosedNTIModel()
			};
		}

		public static NtiModel BuildNTIModel()
		{
			var ntiModel = new NtiModel();
			ntiModel.Id = Fixture.Create<long>();
			ntiModel.CaseUrn = Fixture.Create<long>();
			ntiModel.Notes = Fixture.Create<string>();

			return ntiModel;
		}

		public static NtiModel BuildClosedNTIModel()
		{
			var ntiModel = BuildNTIModel();
			var closedStatusId = Fixture.Create<int>();

			ntiModel.ClosedAt = Fixture.Create<DateTime>();
			ntiModel.ClosedStatusId = closedStatusId;
			ntiModel.ClosedStatus = new NtiStatusModel()
			{
				Id = closedStatusId,
				Name = Fixture.Create<string>()
			};

			return ntiModel;
		}
	}
}