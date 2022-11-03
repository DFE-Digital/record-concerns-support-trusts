using ConcernsCaseWork.Service.FinancialPlan;
using System;

namespace ConcernsCaseWork.Redis.Models
{
	[Serializable]
	public sealed class FinancialPlanWrapper
	{
		public FinancialPlanDto FinancialPlanDto { get;set;}
	}
}
