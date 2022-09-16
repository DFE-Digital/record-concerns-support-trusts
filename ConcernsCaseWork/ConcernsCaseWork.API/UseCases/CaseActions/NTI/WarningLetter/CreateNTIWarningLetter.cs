using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.WarningLetter;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.WarningLetter;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.NTI.WarningLetter
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
