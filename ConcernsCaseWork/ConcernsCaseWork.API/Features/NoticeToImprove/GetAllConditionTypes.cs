using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.NoticeToImprove
{
	public class GetAllConditionTypes : IUseCase<object, List<NoticeToImproveConditionType>>
	{
		private readonly INoticeToImproveGateway _gateway;

		public GetAllConditionTypes(INoticeToImproveGateway gateway)
		{
			_gateway = gateway;
		}

		public List<NoticeToImproveConditionType> Execute(object ignore)
		{
			return ExecuteAsync(ignore).Result;
		}

		public async Task<List<NoticeToImproveConditionType>> ExecuteAsync(object ignore)
		{
			return await _gateway.GetAllConditionTypes();
		}
	}
}
