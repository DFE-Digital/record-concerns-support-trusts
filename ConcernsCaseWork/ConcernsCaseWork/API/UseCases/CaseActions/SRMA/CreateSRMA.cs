using Concerns.Data.Factories.CaseActionFactories;
using Concerns.Data.Gateways;
using Concerns.Data.RequestModels.CaseActions.SRMA;
using Concerns.Data.ResponseModels.CaseActions.SRMA;

namespace Concerns.Data.UseCases.CaseActions.SRMA
{
    public class CreateSRMA : IUseCase<CreateSRMARequest, SRMAResponse>
    {
        private readonly ISRMAGateway _gateway;

        public CreateSRMA(ISRMAGateway gateway)
        {
            _gateway = gateway;
        }

        public SRMAResponse Execute(CreateSRMARequest request)
        {
            return ExecuteAsync(request).Result;
        }

        public async Task<SRMAResponse> ExecuteAsync(CreateSRMARequest request)
        {
            var dbModel = SRMAFactory.CreateDBModel(request);
            var createdSRMA = await _gateway.CreateSRMA(dbModel);

            return SRMAFactory.CreateResponse(createdSRMA);
        }
    }
}
