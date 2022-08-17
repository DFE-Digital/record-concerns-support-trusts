using Concerns.Data.Gateways;
using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.RequestModels.CaseActions.FinancialPlan;
using ConcernsCaseWork.API.ResponseModels.CaseActions.FinancialPlan;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.UseCases.CaseActions.FinancialPlan
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
