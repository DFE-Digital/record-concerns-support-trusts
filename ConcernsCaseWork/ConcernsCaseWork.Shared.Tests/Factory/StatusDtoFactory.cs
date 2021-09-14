using Service.TRAMS.Status;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class StatusDtoFactory
	{
		public static StatusDto BuildStatusDto(string statusName, long urn)
		{
			var currentDate = DateTimeOffset.Now;
			return new StatusDto(statusName, currentDate, currentDate, urn);
		}

		public static List<StatusDto> BuildListStatusDto()
		{
			return new List<StatusDto>
			{
				new StatusDto("Live", DateTime.Now, DateTime.Now, 1),
				new StatusDto("Monitoring", DateTime.Now, DateTime.Now, 2),
				new StatusDto("Close", DateTime.Now, DateTime.Now, 3)
			};
		}
	}
}