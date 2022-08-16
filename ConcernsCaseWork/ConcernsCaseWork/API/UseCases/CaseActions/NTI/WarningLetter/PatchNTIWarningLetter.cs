using Concerns.Data.Factories.CaseActionFactories;
using Concerns.Data.Gateways;
using Concerns.Data.RequestModels.CaseActions.NTI.WarningLetter;
using Concerns.Data.ResponseModels.CaseActions.NTI.WarningLetter;

namespace Concerns.Data.UseCases.CaseActions.NTI.WarningLetter
{
    public class PatchNTIWarningLetter : IUseCase<PatchNTIWarningLetterRequest, NTIWarningLetterResponse>
    {
        private readonly INTIWarningLetterGateway _gateway;

        public PatchNTIWarningLetter(INTIWarningLetterGateway gateway)
        {
            _gateway = gateway;
        }

        public NTIWarningLetterResponse Execute(PatchNTIWarningLetterRequest request)
        {
            return ExecuteAsync(request).Result;
        }

        public async Task<NTIWarningLetterResponse> ExecuteAsync(PatchNTIWarningLetterRequest request)
        {
            var patchedNTIWarningLetter = await _gateway.PatchNTIWarningLetter(NTIWarningLetterFactory.CreateDBModel(request));
            return NTIWarningLetterFactory.CreateResponse(patchedNTIWarningLetter);
        }
    }
}
