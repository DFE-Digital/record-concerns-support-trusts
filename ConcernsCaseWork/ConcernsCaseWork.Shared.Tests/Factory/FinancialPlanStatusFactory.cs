using ConcernsCaseWork.Enums;
using Service.TRAMS.FinancialPlan;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class FinancialPlanStatusFactory
	{
		public static List<FinancialPlanStatusDto> BuildListAllFinancialPlanStatusDto()
		{
			return new List<FinancialPlanStatusDto>
			{
				BuildFinancialPlanStatusDto(FinancialPlanStatus.AwaitingPlan, false),
				BuildFinancialPlanStatusDto(FinancialPlanStatus.ReturnToTrust,false),
				BuildFinancialPlanStatusDto(FinancialPlanStatus.ViablePlanReceived, true),
				BuildFinancialPlanStatusDto(FinancialPlanStatus.Abandoned, true)
			};
		}
		
		public static List<FinancialPlanStatusDto> BuildListOpenFinancialPlanStatusDto()
			=> BuildListAllFinancialPlanStatusDto().Where(d => d.IsClosedStatus).ToList();
		
		public static List<FinancialPlanStatusDto> BuildListClosureFinancialPlanStatusDto()
			=> BuildListAllFinancialPlanStatusDto().Where(d => !d.IsClosedStatus).ToList();

		private static FinancialPlanStatusDto BuildFinancialPlanStatusDto(FinancialPlanStatus financialPlanStatus, bool isClosedStatus)
			=> new FinancialPlanStatusDto(
				financialPlanStatus.ToString(), 
				financialPlanStatus.ToString().ToUpper(),
				(int)financialPlanStatus, 
				isClosedStatus, 
				DateTime.Now, DateTime.Now);
	}
}