using ConcernsCaseWork.API.Contracts.NoticeToImprove;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.NoticeToImprove
{
	public class GetNoticeToImproveById : IUseCase<long, NoticeToImproveResponse>
	{
		private readonly INoticeToImproveGateway _gateway;

		public GetNoticeToImproveById(INoticeToImproveGateway gateway)
		{
			_gateway = gateway;
		}

		public NoticeToImproveResponse Execute(long noticeToImproveId)
		{
			return ExecuteAsync(noticeToImproveId).Result;
		}

		public async Task<NoticeToImproveResponse> ExecuteAsync(long noticeToImproveId)
		{
			var noticeToImprove = await _gateway.GetNoticeToImproveById(noticeToImproveId);
			if (noticeToImprove == null)
				return null;
			return NoticeToImproveFactory.CreateResponse(noticeToImprove);
		}
	}
}
