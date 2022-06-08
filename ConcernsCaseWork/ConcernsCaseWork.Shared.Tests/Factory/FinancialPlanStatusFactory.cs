using Service.TRAMS.FinancialPlan;
using Service.TRAMS.Status;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class FinancialPlanStatusFactory
	{
		public static StatusDto BuildFinancialPlanStatusDto(string statusName, long urn)
		{
			var currentDate = DateTimeOffset.Now;
			return new StatusDto(statusName, currentDate, currentDate, urn);
		}

		public static List<FinancialPlanStatusDto> BuildListFinancialPlanStatusDto()
		{
			return new List<FinancialPlanStatusDto>
			{
				new FinancialPlanStatusDto("AwaitingPlan", DateTime.Now, DateTime.Now, 1),
				new FinancialPlanStatusDto("ReturnToTrust", DateTime.Now, DateTime.Now, 2),
				new FinancialPlanStatusDto("ViablePlanReceived", DateTime.Now, DateTime.Now, 3),
				new FinancialPlanStatusDto("Abandoned", DateTime.Now, DateTime.Now, 4)
			};
		}
	}
}