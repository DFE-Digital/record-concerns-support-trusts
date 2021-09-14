using Service.TRAMS.Status;
using System;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class StatusDtoFactory
	{
		public static StatusDto BuildStatusDto(string statusName, long urn)
		{
			var currentDate = DateTimeOffset.Now;
			return new StatusDto(statusName, currentDate, currentDate, urn);
		}
	}
}