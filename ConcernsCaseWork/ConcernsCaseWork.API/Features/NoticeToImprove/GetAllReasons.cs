using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.NoticeToImprove
{
	public class GetAllReasons : IUseCase<object, List<NoticeToImproveReason>>
	{
		private readonly INoticeToImproveGateway _gateway;

		public GetAllReasons(INoticeToImproveGateway gateway)
		{
			_gateway = gateway;
		}

		public List<NoticeToImproveReason> Execute(object ignore)
		{
			return ExecuteAsync(ignore).Result;
		}

		public async Task<List<NoticeToImproveReason>> ExecuteAsync(object ignore)
		{
			return await _gateway.GetAllReasons();
		}
	}
}
