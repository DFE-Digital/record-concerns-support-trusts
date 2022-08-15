using Concerns.Data.Factories.CaseActionFactories;
using Concerns.Data.Gateways;
using Concerns.Data.RequestModels.CaseActions.NTI.WarningLetter;
using Concerns.Data.ResponseModels.CaseActions.NTI.WarningLetter;

namespace Concerns.Data.UseCases.CaseActions.NTI.WarningLetter
{
    public class CreateNTIWarningLetter : IUseCase<CreateNTIWarningLetterRequest, NTIWarningLetterResponse>
    {
        private readonly INTIWarningLetterGateway _gateway;

        public CreateNTIWarningLetter(INTIWarningLetterGateway gateway)
        {
            _gateway = gateway;
        }

        public NTIWarningLetterResponse Execute(CreateNTIWarningLetterRequest request)
        {
            return ExecuteAsync(request).Result;
        }

        public async Task<NTIWarningLetterResponse> ExecuteAsync(CreateNTIWarningLetterRequest request)
        {
            var dbModel = NTIWarningLetterFactory.CreateDBModel(request);

            var createdNTIWarningLetter = await _gateway.CreateNTIWarningLetter(dbModel);

            return NTIWarningLetterFactory.CreateResponse(createdNTIWarningLetter);
        }

    }
}
