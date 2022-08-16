using Concerns.Data.Factories.CaseActionFactories;
using Concerns.Data.Gateways;
using Concerns.Data.RequestModels.CaseActions.NTI.UnderConsideration;
using Concerns.Data.ResponseModels.CaseActions.NTI.UnderConsideration;

namespace Concerns.Data.UseCases.CaseActions.NTI.UnderConsideration
{
    public class CreateNTIUnderConsideration : IUseCase<CreateNTIUnderConsiderationRequest, NTIUnderConsiderationResponse>
    {
        private readonly INTIUnderConsiderationGateway _gateway;

        public CreateNTIUnderConsideration(INTIUnderConsiderationGateway gateway)
        {
            _gateway = gateway;
        }

        public NTIUnderConsiderationResponse Execute(CreateNTIUnderConsiderationRequest request)
        {
            return ExecuteAsync(request).Result;
        }

        public async Task<NTIUnderConsiderationResponse> ExecuteAsync(CreateNTIUnderConsiderationRequest request)
        {
            var dbModel = NTIUnderConsiderationFactory.CreateDBModel(request);

            var createdNTIUnderConsideration = await _gateway.CreateNTIUnderConsideration(dbModel);

            return NTIUnderConsiderationFactory.CreateResponse(createdNTIUnderConsideration);
        }

    }
}
