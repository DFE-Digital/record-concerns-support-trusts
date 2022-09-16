using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.Data.Gateways
{
    public interface IFinancialPlanGateway
    {
        Task<FinancialPlanCase> CreateFinancialPlan(FinancialPlanCase request);
        Task<FinancialPlanCase> GetFinancialPlanById(long financialPlanId);
        Task<ICollection<FinancialPlanCase>> GetFinancialPlansByCaseUrn(int caseUrn);
        Task<FinancialPlanCase> PatchFinancialPlan(FinancialPlanCase updatedFinancialPlan);
        Task<List<FinancialPlanStatus>> GetAllStatuses();

    }
}
