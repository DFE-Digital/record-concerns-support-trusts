using Concerns.Data.Factories.CaseActionFactories;
using Concerns.Data.Gateways;
using Concerns.Data.ResponseModels.CaseActions.NTI.UnderConsideration;

namespace Concerns.Data.UseCases.CaseActions.NTI.UnderConsideration
{
    public class GetNTIUnderConsiderationById : IUseCase<long, NTIUnderConsiderationResponse>
    {
        private readonly INTIUnderConsiderationGateway _gateway;

        public GetNTIUnderConsiderationById(INTIUnderConsiderationGateway gateway)
        {
            _gateway = gateway;
        }

        public NTIUnderConsiderationResponse Execute(long underConsiderationId)
        {
            return ExecuteAsync(underConsiderationId).Result;
        }

        public async Task<NTIUnderConsiderationResponse> ExecuteAsync(long underConsiderationId)
        {
            var consideration = await _gateway.GetNTIUnderConsiderationById(underConsiderationId);
            return NTIUnderConsiderationFactory.CreateResponse(consideration);
        }
    }
}
