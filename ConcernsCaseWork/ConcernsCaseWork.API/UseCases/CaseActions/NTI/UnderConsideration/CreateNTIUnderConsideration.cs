using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.UnderConsideration;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.UnderConsideration;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.NTI.UnderConsideration
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
