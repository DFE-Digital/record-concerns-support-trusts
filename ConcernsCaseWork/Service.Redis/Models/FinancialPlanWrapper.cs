using ConcernsCasework.Service.FinancialPlan;
using System;

namespace Service.Redis.Models
{
	[Serializable]
	public sealed class FinancialPlanWrapper
	{
		public FinancialPlanDto FinancialPlanDto { get;set;}
	}
}
