using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.NTIUnderConsideration
{
	public class GetAllStatuses : IUseCase<object, List<NTIUnderConsiderationStatus>>
	{
		private readonly INTIUnderConsiderationGateway _gateway;

		public GetAllStatuses(INTIUnderConsiderationGateway gateway)
		{
			_gateway = gateway;
		}

		public List<NTIUnderConsiderationStatus> Execute(object ignore)
		{
			return ExecuteAsync(ignore).Result;
		}

		public async Task<List<NTIUnderConsiderationStatus>> ExecuteAsync(object ignore)
		{
			return await _gateway.GetAllStatuses();
		}
	}
}
