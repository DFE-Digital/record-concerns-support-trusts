using Concerns.Data.Gateways;
using Concerns.Data.Models;

namespace Concerns.Data.UseCases.CaseActions.NTI.WarningLetter
{
    public class GetAllStatuses : IUseCase<Object, List<NTIWarningLetterStatus>>
    {
        private readonly INTIWarningLetterGateway _gateway;

        public GetAllStatuses(INTIWarningLetterGateway gateway)
        {
            _gateway = gateway;
        }

        public List<NTIWarningLetterStatus> Execute(Object ignore)
        {
            return ExecuteAsync(ignore).Result;
        }

        public async Task<List<NTIWarningLetterStatus>> ExecuteAsync(Object ignore)
        {
            return await _gateway.GetAllStatuses();
        }
    }
}
