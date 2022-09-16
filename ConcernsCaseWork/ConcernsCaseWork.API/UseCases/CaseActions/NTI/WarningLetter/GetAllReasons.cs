using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.NTI.WarningLetter
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
