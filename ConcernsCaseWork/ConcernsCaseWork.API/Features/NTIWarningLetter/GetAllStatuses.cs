using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.NTIWarningLetter
{
	public class GetAllStatuses : IUseCase<object, List<NTIWarningLetterStatus>>
	{
		private readonly INTIWarningLetterGateway _gateway;

		public GetAllStatuses(INTIWarningLetterGateway gateway)
		{
			_gateway = gateway;
		}

		public List<NTIWarningLetterStatus> Execute(object ignore)
		{
			return ExecuteAsync(ignore).Result;
		}

		public async Task<List<NTIWarningLetterStatus>> ExecuteAsync(object ignore)
		{
			return await _gateway.GetAllStatuses();
		}
	}
}
