using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.NoticeToImprove
{
	public class GetAllStatuses : IUseCase<object, List<NoticeToImproveStatus>>
	{
		private readonly INoticeToImproveGateway _gateway;

		public GetAllStatuses(INoticeToImproveGateway gateway)
		{
			_gateway = gateway;
		}

		public List<NoticeToImproveStatus> Execute(object ignore)
		{
			return ExecuteAsync(ignore).Result;
		}

		public async Task<List<NoticeToImproveStatus>> ExecuteAsync(object ignore)
		{
			return await _gateway.GetAllStatuses();
		}
	}
}
