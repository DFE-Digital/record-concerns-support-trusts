using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.FinancialPlan
{
	public interface IFinancialPlanStatusService
	{
		Task<IList<FinancialPlanStatusDto>> GetFinancialPlansStatuses();
	}
}
