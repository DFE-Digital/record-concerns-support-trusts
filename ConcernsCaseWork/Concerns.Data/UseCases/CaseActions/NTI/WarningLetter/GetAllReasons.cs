using Concerns.Data.Gateways;
using Concerns.Data.Models;

namespace Concerns.Data.UseCases.CaseActions.NTI.WarningLetter
{
    public class GetAllReasons : IUseCase<Object, List<NTIWarningLetterReason>>
    {
        private readonly INTIWarningLetterGateway _gateway;

        public GetAllReasons(INTIWarningLetterGateway gateway)
        {
            _gateway = gateway;
        }

        public List<NTIWarningLetterReason> Execute(Object ignore)
        {
            return ExecuteAsync(ignore).Result;
        }

        public async Task<List<NTIWarningLetterReason>> ExecuteAsync(Object ignore)
        {
            return await _gateway.GetAllReasons();
        }
    }
}
