using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.ResponseModels.CaseActions.FinancialPlan;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.FinancialPlan
{
    public class GetFinancialPlanById : IUseCase<long, FinancialPlanResponse>
    {
        private readonly IFinancialPlanGateway _gateway;

        public GetFinancialPlanById(IFinancialPlanGateway gateway)
        {
            _gateway = gateway;
        }

        public FinancialPlanResponse Execute(long financialPlanId)
        {
            return ExecuteAsync(financialPlanId).Result;
        }

        public async Task<FinancialPlanResponse> ExecuteAsync(long financialPlanId)
        {
            var fp = await _gateway.GetFinancialPlanById(financialPlanId);
            return FinancialPlanFactory.CreateResponse(fp);
        }
    }
}
