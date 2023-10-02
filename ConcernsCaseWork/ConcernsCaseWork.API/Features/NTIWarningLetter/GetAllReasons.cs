using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.NTIWarningLetter
{
	public class GetAllReasons : IUseCase<object, List<NTIWarningLetterReason>>
	{
		private readonly INTIWarningLetterGateway _gateway;

		public GetAllReasons(INTIWarningLetterGateway gateway)
		{
			_gateway = gateway;
		}

		public List<NTIWarningLetterReason> Execute(object ignore)
		{
			return ExecuteAsync(ignore).Result;
		}

		public async Task<List<NTIWarningLetterReason>> ExecuteAsync(object ignore)
		{
			return await _gateway.GetAllReasons();
		}
	}
}
