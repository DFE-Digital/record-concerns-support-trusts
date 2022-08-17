using Concerns.Data.Gateways;
using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.RequestModels.CaseActions.FinancialPlan;
using ConcernsCaseWork.API.ResponseModels.CaseActions.FinancialPlan;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.UseCases.CaseActions.FinancialPlan
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
