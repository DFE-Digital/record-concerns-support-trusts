namespace ConcernsCaseWork.Service.FinancialPlan
{
	public interface IFinancialPlanService
	{
		Task<IList<FinancialPlanDto>> GetFinancialPlansByCaseUrn(long caseUrn);
		Task<FinancialPlanDto> GetFinancialPlansById(long caseUrn, long financialPlanId);
		Task<FinancialPlanDto> PostFinancialPlanByCaseUrn(CreateFinancialPlanDto createFinancialPlanDto);
		Task<FinancialPlanDto> PatchFinancialPlanById(FinancialPlanDto financialPlanDto);
	}
}
