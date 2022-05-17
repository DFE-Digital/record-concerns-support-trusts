using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.FinancialPlan
{
	public interface IFinancialPlanService
	{
		Task<IList<FinancialPlanDto>> GetFinancialPlansByCaseUrn(long caseUrn);
		Task<FinancialPlanDto> PostFinancialPlanByCaseUrn(CreateFinancialPlanDto createFinancialPlanDto);
		Task<FinancialPlanDto> PatchFinancialPlanById(FinancialPlanDto financialPlanDto);
	}
}
