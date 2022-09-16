using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.WarningLetter;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.NTI.WarningLetter
{
    public class GetNTIWarningLetterById : IUseCase<long, NTIWarningLetterResponse>
    {
        private readonly INTIWarningLetterGateway _gateway;

        public GetNTIWarningLetterById(INTIWarningLetterGateway gateway)
        {
            _gateway = gateway;
        }

        public NTIWarningLetterResponse Execute(long warningLetterId)
        {
            return ExecuteAsync(warningLetterId).Result;
        }

        public async Task<NTIWarningLetterResponse> ExecuteAsync(long warningLetterId)
        {
            var warningLetter = await _gateway.GetNTIWarningLetterById(warningLetterId);
            return NTIWarningLetterFactory.CreateResponse(warningLetter);
        }
    }
}
