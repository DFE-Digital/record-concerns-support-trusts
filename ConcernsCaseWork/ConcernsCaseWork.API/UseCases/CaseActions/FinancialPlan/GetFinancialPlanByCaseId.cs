using Concerns.Data.Gateways;
using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.ResponseModels.CaseActions.FinancialPlan;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.UseCases.CaseActions.FinancialPlan
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
