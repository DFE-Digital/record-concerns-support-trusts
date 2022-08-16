using Concerns.Data.Factories.CaseActionFactories;
using Concerns.Data.Gateways;
using Concerns.Data.ResponseModels.CaseActions.SRMA;

namespace Concerns.Data.UseCases.CaseActions.SRMA
{
    public class GetSRMAsByCaseId : IUseCase<int, ICollection<SRMAResponse>>
    {
        private readonly ISRMAGateway _gateway;

        public GetSRMAsByCaseId(ISRMAGateway gateway)
        {
            _gateway = gateway;
        }

        public ICollection<SRMAResponse> Execute(int caseId)
        {
            return ExecuteAsync(caseId).Result;
        }

        public async Task<ICollection<SRMAResponse>> ExecuteAsync(int caseId)
        {
            var dbModels = await _gateway.GetSRMAsByCaseId(caseId);
            return dbModels.Select(dbm => SRMAFactory.CreateResponse(dbm)).ToList();
        }

    }
}
