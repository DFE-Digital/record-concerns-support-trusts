using Concerns.Data.Gateways;
using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.UnderConsideration;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.UnderConsideration;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.UseCases.CaseActions.NTI.UnderConsideration
{
    public class PatchNTIUnderConsideration : IUseCase<PatchNTIUnderConsiderationRequest, NTIUnderConsiderationResponse>
    {
        private readonly INTIUnderConsiderationGateway _gateway;

        public PatchNTIUnderConsideration(INTIUnderConsiderationGateway gateway)
        {
            _gateway = gateway;
        }

        public NTIUnderConsiderationResponse Execute(PatchNTIUnderConsiderationRequest request)
        {
            return ExecuteAsync(request).Result;
        }

        public async Task<NTIUnderConsiderationResponse> ExecuteAsync(PatchNTIUnderConsiderationRequest request)
        {
            var patchedSRMA = await _gateway.PatchNTIUnderConsideration(NTIUnderConsiderationFactory.CreateDBModel(request));
            return NTIUnderConsiderationFactory.CreateResponse(patchedSRMA);
        }
    }
}
