using Concerns.Data.Factories.CaseActionFactories;
using Concerns.Data.Gateways;
using Concerns.Data.RequestModels.CaseActions.SRMA;
using Concerns.Data.ResponseModels.CaseActions.SRMA;

namespace Concerns.Data.UseCases.CaseActions.SRMA
{
    public class PatchSRMA : IUseCase<PatchSRMARequest, SRMAResponse>
    {
        private readonly ISRMAGateway _gateway;

        public PatchSRMA(ISRMAGateway gateway)
        {
            _gateway = gateway;
        }

        public SRMAResponse Execute(PatchSRMARequest request)
        {
            return ExecuteAsync(request).Result;
        }

        public async Task<SRMAResponse> ExecuteAsync(PatchSRMARequest request)
        {
            if (request?.Delegate == null || request.SRMAId == default(int))
            {
                throw new ArgumentNullException("SRMA Id or delegate not provided");
            }

            var patchedSRMA = await _gateway.PatchSRMAAsync(request.SRMAId, request.Delegate);
            return SRMAFactory.CreateResponse(patchedSRMA);
        }
    }
}
