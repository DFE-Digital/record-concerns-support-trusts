using ConcernsCaseWork.Models.CaseActions;
using Service.Redis.Models;
using ConcernsCasework.Service.FinancialPlan;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.FinancialPlan
{
	public interface IFinancialPlanModelService
	{
		public Task<IList<FinancialPlanModel>> GetFinancialPlansModelByCaseUrn(long caseUrn, string caseworker);
		public Task<FinancialPlanModel> GetFinancialPlansModelById(long caseUrn, long financialPlanId ,string caseworker);

		public Task<FinancialPlanDto> PostFinancialPlanByCaseUrn(CreateFinancialPlanModel financialPlan, string caseworker);

		public Task PatchFinancialById(PatchFinancialPlanModel patchFinancialPlanModel, string caseworker);
	}
}
