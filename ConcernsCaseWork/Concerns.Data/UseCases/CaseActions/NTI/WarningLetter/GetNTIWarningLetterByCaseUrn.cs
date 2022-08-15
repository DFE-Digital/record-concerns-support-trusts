using Concerns.Data.Factories.CaseActionFactories;
using Concerns.Data.Gateways;
using Concerns.Data.ResponseModels.CaseActions.NTI.WarningLetter;

namespace Concerns.Data.UseCases.CaseActions.NTI.WarningLetter
{
    public class GetNTIWarningLetterByCaseUrn : IUseCase<int, List<NTIWarningLetterResponse>>
    {
        private readonly INTIWarningLetterGateway _gateway;

        public GetNTIWarningLetterByCaseUrn(INTIWarningLetterGateway gateway)
        {
            _gateway = gateway;
        }

        public List<NTIWarningLetterResponse> Execute(int caseUrn)
        {
            return ExecuteAsync(caseUrn).Result;
        }

        public async Task<List<NTIWarningLetterResponse>> ExecuteAsync(int caseUrn)
        {
            var warningLetters = await _gateway.GetNTIWarningLetterByCaseUrn(caseUrn);
            return warningLetters.Select(warningLetter => NTIWarningLetterFactory.CreateResponse(warningLetter)).ToList();
        }
    }
}