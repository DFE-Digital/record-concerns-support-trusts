namespace ConcernsCaseWork.Service.FinancialPlan
{
	public interface IFinancialPlanStatusService
	{
		Task<IList<FinancialPlanStatusDto>> GetAllFinancialPlansStatusesAsync();
		Task<IList<FinancialPlanStatusDto>> GetClosureFinancialPlansStatusesAsync();
		Task<IList<FinancialPlanStatusDto>> GetOpenFinancialPlansStatusesAsync();
	}
}
