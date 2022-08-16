using Concerns.Data.Factories.CaseActionFactories;
using Concerns.Data.Gateways;
using Concerns.Data.RequestModels.CaseActions.FinancialPlan;
using Concerns.Data.ResponseModels.CaseActions.FinancialPlan;

namespace Concerns.Data.UseCases.CaseActions.FinancialPlan
{
    public class PatchFinancialPlan : IUseCase<PatchFinancialPlanRequest, FinancialPlanResponse>
    {
        private readonly IFinancialPlanGateway _gateway;

        public PatchFinancialPlan(IFinancialPlanGateway gateway)
        {
            _gateway = gateway;
        }

        public FinancialPlanResponse Execute(PatchFinancialPlanRequest request)
        {
            return ExecuteAsync(request).Result;
        }

        public async Task<FinancialPlanResponse> ExecuteAsync(PatchFinancialPlanRequest request)
        {
            var patchedSRMA = await _gateway.PatchFinancialPlan(FinancialPlanFactory.CreateDBModel(request));
            return FinancialPlanFactory.CreateResponse(patchedSRMA);
        }
    }
}
