using AutoFixture;
using Service.TRAMS.Nti;
using Service.TRAMS.Status;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class NTIStatusFactory
	{
		private readonly static Fixture Fixture = new Fixture();

		public static NtiStatusDto BuildNTIStatusDto(int? id, bool isClosingState = false)
		{
			var currentDate = DateTimeOffset.Now;

			var status = new NtiStatusDto();
			status.Id = id ?? Fixture.Create<int>();
			status.Name = Fixture.Create<string>();
			status.Description = Fixture.Create<string>();
			status.IsClosingState = isClosingState;
			status.CreatedAt = currentDate;
			status.UpdatedAt = currentDate;

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