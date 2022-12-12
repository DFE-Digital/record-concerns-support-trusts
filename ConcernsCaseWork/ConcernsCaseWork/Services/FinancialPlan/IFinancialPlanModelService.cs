using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Service.FinancialPlan;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.FinancialPlan
{
	public interface IFinancialPlanModelService
	{
		public Task<IList<FinancialPlanModel>> GetFinancialPlansModelByCaseUrn(long caseUrn);
		public Task<FinancialPlanModel> GetFinancialPlansModelById(long caseUrn, long financialPlanId);

		public Task<FinancialPlanDto> PostFinancialPlanByCaseUrn(CreateFinancialPlanModel financialPlan);

		public Task PatchFinancialById(PatchFinancialPlanModel patchFinancialPlanModel);
	}
}
