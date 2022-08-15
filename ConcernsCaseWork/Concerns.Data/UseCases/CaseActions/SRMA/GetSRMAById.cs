using Concerns.Data.Factories.CaseActionFactories;
using Concerns.Data.Gateways;
using Concerns.Data.ResponseModels.CaseActions.SRMA;

namespace Concerns.Data.UseCases.CaseActions.SRMA
{
    public class GetSRMAById : IUseCase<int, SRMAResponse>
    {
        private readonly ISRMAGateway _gateway;

        public GetSRMAById(ISRMAGateway gateway)
        {
            _gateway = gateway;
        }

        public SRMAResponse Execute(int srmaId)
        {
            return ExecuteAsync(srmaId).Result;
        }
        public async Task<SRMAResponse> ExecuteAsync(int srmaId)
        {
            var srma = await _gateway.GetSRMAById(srmaId);
            return SRMAFactory.CreateResponse(srma);
        }
    }
}
