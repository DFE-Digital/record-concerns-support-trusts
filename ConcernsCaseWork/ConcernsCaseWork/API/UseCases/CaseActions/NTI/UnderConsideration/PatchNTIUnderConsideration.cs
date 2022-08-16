using Concerns.Data.Factories.CaseActionFactories;
using Concerns.Data.Gateways;
using Concerns.Data.RequestModels.CaseActions.NTI.UnderConsideration;
using Concerns.Data.ResponseModels.CaseActions.NTI.UnderConsideration;

namespace Concerns.Data.UseCases.CaseActions.NTI.UnderConsideration
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
