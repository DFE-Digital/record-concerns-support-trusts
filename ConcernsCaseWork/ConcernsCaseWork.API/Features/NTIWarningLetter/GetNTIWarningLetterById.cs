using ConcernsCaseWork.API.Contracts.NtiWarningLetter;
using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.NTIWarningLetter
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
			if (warningLetter == null)
				return null;
			return NTIWarningLetterFactory.CreateResponse(warningLetter);
		}
	}
}
