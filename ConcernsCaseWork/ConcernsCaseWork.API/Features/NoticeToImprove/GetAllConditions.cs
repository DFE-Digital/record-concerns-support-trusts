using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.NoticeToImprove
{
	public class GetAllConditions : IUseCase<object, List<NoticeToImproveCondition>>
	{
		private readonly INoticeToImproveGateway _gateway;

		public GetAllConditions(INoticeToImproveGateway gateway)
		{
			_gateway = gateway;
		}

		public List<NoticeToImproveCondition> Execute(object ignore)
		{
			return ExecuteAsync(ignore).Result;
		}

		public async Task<List<NoticeToImproveCondition>> ExecuteAsync(object ignore)
		{
			return await _gateway.GetAllConditions();
		}
	}
}
