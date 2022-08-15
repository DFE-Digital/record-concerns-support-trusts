using Concerns.Data.Gateways;
using Concerns.Data.Models;

namespace Concerns.Data.UseCases.CaseActions.NTI.UnderConsideration
{
    public class GetAllReasons : IUseCase<Object, List<NTIUnderConsiderationReason>>
    {
        private readonly INTIUnderConsiderationGateway _gateway;

        public GetAllReasons(INTIUnderConsiderationGateway gateway)
        {
            _gateway = gateway;
        }

        public List<NTIUnderConsiderationReason> Execute(Object ignore)
        {
            return ExecuteAsync(ignore).Result;
        }

        public async Task<List<NTIUnderConsiderationReason>> ExecuteAsync(Object ignore)
        {
            return await _gateway.GetAllReasons();
        }
    }
}
