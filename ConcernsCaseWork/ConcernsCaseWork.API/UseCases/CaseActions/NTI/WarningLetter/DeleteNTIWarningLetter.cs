using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.NTI.WarningLetter
{
	public class DeleteNTIWarningLetter : IUseCase<long, DeleteNTIWarningLetterResponse>
	{
		private readonly INTIWarningLetterGateway _gateway;

		public DeleteNTIWarningLetter(INTIWarningLetterGateway gateway)
		{
			_gateway = gateway;
		}

		public DeleteNTIWarningLetterResponse Execute(long warningLetterId)
		{
			_gateway.Delete(warningLetterId);
			return new DeleteNTIWarningLetterResponse();
		}
	}

	public class DeleteNTIWarningLetterResponse
	{
	}
}
