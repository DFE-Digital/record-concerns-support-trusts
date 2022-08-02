using Service.TRAMS.FinancialPlan;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.FinancialPlan
{
	public interface IFinancialPlanStatusCachedService
	{
		Task ClearData();
		Task<IList<FinancialPlanStatusDto>> GetAllFinancialPlanStatuses();
		Task<IList<FinancialPlanStatusDto>> GetClosureFinancialPlansStatuses();
		Task<IList<FinancialPlanStatusDto>> GetOpenFinancialPlansStatuses();
	}
}