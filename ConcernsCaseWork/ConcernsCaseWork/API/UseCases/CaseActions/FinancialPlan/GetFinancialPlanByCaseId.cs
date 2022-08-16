using Concerns.Data.Factories.CaseActionFactories;
using Concerns.Data.Gateways;
using Concerns.Data.ResponseModels.CaseActions.FinancialPlan;

namespace Concerns.Data.UseCases.CaseActions.FinancialPlan
{
    public class GetFinancialPlanByCaseId : IUseCase<int, List<FinancialPlanResponse>>
    {
        private readonly IFinancialPlanGateway _gateway;

        public GetFinancialPlanByCaseId(IFinancialPlanGateway gateway)
        {
            _gateway = gateway;
        }

        public List<FinancialPlanResponse> Execute(int caseUrn)
        {
            return ExecuteAsync(caseUrn).Result;
        }
        public async Task<List<FinancialPlanResponse>> ExecuteAsync(int caseUrn)
        {
            var fps = await _gateway.GetFinancialPlansByCaseUrn(caseUrn);
            return fps.Select(fp => FinancialPlanFactory.CreateResponse(fp)).ToList();
        }
    }
}
