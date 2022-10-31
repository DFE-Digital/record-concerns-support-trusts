using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.UnderConsideration;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.NTI.UnderConsideration
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
