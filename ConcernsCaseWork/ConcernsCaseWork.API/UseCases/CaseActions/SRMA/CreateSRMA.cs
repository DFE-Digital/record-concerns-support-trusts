using Concerns.Data.Gateways;
using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.RequestModels.CaseActions.SRMA;
using ConcernsCaseWork.API.ResponseModels.CaseActions.SRMA;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.UseCases.CaseActions.SRMA
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
