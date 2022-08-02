using ConcernsCaseWork.Enums;
using Service.TRAMS.FinancialPlan;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class FinancialPlanStatusFactory
	{
		public static List<FinancialPlanStatusDto> BuildListFinancialPlanStatusDto()
		{
			return new List<FinancialPlanStatusDto>
			{
				BuildFinancialPlanStatusDto(FinancialPlanStatus.AwaitingPlan, false),
				BuildFinancialPlanStatusDto(FinancialPlanStatus.ReturnToTrust,false),
				BuildFinancialPlanStatusDto(FinancialPlanStatus.ViablePlanReceived, true),
				BuildFinancialPlanStatusDto(FinancialPlanStatus.Abandoned, true)
			};
		}

		private static FinancialPlanStatusDto BuildFinancialPlanStatusDto(FinancialPlanStatus financialPlanStatus, bool isClosedStatus)
			=> new FinancialPlanStatusDto(financialPlanStatus.ToString(), financialPlanStatus.ToString().ToUpper(), (int)financialPlanStatus, isClosedStatus);
	}
}