using Concerns.Data.Gateways;
using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.WarningLetter;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.WarningLetter;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.UseCases.CaseActions.NTI.WarningLetter
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
