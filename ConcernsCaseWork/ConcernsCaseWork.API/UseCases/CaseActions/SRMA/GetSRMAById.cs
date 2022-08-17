using Concerns.Data.Gateways;
using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.ResponseModels.CaseActions.SRMA;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.UseCases.CaseActions.SRMA
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
