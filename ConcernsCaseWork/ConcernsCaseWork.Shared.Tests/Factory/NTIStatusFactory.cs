using AutoFixture;
using ConcernsCasework.Service.Nti;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class NTIStatusFactory
	{
		private readonly static Fixture Fixture = new Fixture();

		public static NtiStatusDto BuildNTIStatusDto(int? id, bool isClosingState = false)
		{
			var status = Fixture.Create<NtiStatusDto>();
			status.Id = id ?? status.Id;

			return status;
		}

		public static List<NtiStatusDto> BuildListNTIStatusDto()
		{
			return new List<NtiStatusDto>
			{
				BuildNTIStatusDto(1),
				BuildNTIStatusDto(2),
				BuildNTIStatusDto(3)
			};
		}
	}
}