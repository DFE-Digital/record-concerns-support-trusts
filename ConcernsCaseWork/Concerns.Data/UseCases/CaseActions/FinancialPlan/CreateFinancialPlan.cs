using Concerns.Data.Factories.CaseActionFactories;
using Concerns.Data.Gateways;
using Concerns.Data.RequestModels.CaseActions.FinancialPlan;
using Concerns.Data.ResponseModels.CaseActions.FinancialPlan;

namespace Concerns.Data.UseCases.CaseActions.FinancialPlan
{
    public class CreateFinancialPlan : IUseCase<CreateFinancialPlanRequest, FinancialPlanResponse>
    {
        private readonly IFinancialPlanGateway _gateway;

        public CreateFinancialPlan(IFinancialPlanGateway financialPlanGateway)
        {
            _gateway = financialPlanGateway;
        }

        public FinancialPlanResponse Execute(CreateFinancialPlanRequest request)
        {
            return ExecuteAsync(request).Result;
        }

        public async Task<FinancialPlanResponse> ExecuteAsync(CreateFinancialPlanRequest request)
        {
            var dbModel = FinancialPlanFactory.CreateDBModel(request);
            var createdSRMA = await _gateway.CreateFinancialPlan(dbModel);

            return FinancialPlanFactory.CreateResponse(createdSRMA);
        }
    }
}
