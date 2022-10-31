using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.ResponseModels.CaseActions.SRMA;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.SRMA
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
