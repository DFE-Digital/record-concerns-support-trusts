using Concerns.Data.Gateways;
using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.UnderConsideration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.UseCases.CaseActions.NTI.UnderConsideration
{
    public class GetNTIUnderConsiderationByCaseUrn : IUseCase<int, List<NTIUnderConsiderationResponse>>
    {
        private readonly INTIUnderConsiderationGateway _gateway;

        public GetNTIUnderConsiderationByCaseUrn(INTIUnderConsiderationGateway gateway)
        {
            _gateway = gateway;
        }

        public List<NTIUnderConsiderationResponse> Execute(int caseUrn)
        {
            return ExecuteAsync(caseUrn).Result;
        }

        public async Task<List<NTIUnderConsiderationResponse>> ExecuteAsync(int caseUrn)
        {
            var considerations = await _gateway.GetNTIUnderConsiderationByCaseUrn(caseUrn);
            return considerations.Select(consideration => NTIUnderConsiderationFactory.CreateResponse(consideration)).ToList();
        }
    }
}