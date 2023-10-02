using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.NTIUnderConsideration
{
	public class GetAllReasons : IUseCase<object, List<NTIUnderConsiderationReason>>
	{
		private readonly INTIUnderConsiderationGateway _gateway;

		public GetAllReasons(INTIUnderConsiderationGateway gateway)
		{
			_gateway = gateway;
		}

		public List<NTIUnderConsiderationReason> Execute(object ignore)
		{
			return ExecuteAsync(ignore).Result;
		}

		public async Task<List<NTIUnderConsiderationReason>> ExecuteAsync(object ignore)
		{
			return await _gateway.GetAllReasons();
		}
	}
}
