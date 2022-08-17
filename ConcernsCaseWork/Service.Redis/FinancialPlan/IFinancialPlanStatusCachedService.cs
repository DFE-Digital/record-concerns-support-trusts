using ConcernsCasework.Service.FinancialPlan;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.FinancialPlan
{
	public interface IFinancialPlanStatusCachedService
	{
		Task ClearData();
		Task<IList<FinancialPlanStatusDto>> GetAllFinancialPlanStatusesAsync();
		Task<IList<FinancialPlanStatusDto>> GetClosureFinancialPlansStatusesAsync();
		Task<IList<FinancialPlanStatusDto>> GetOpenFinancialPlansStatusesAsync();
	}
}