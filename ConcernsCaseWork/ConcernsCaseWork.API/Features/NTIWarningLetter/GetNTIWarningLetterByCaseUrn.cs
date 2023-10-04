using ConcernsCaseWork.API.Contracts.NtiWarningLetter;
using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.NTIWarningLetter
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