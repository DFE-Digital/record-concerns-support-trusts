using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.NoticeToImprove
{
	public class DeleteNoticeToImprove : IUseCase<long, DeleteNoticeToImproveResponse>
	{
		private readonly INoticeToImproveGateway _gateway;


		public DeleteNoticeToImprove(INoticeToImproveGateway gateway)
		{
			_gateway = gateway;
		}

		public DeleteNoticeToImproveResponse Execute(long warningLetterId)
		{
			_gateway.Delete(warningLetterId);
			return new DeleteNoticeToImproveResponse();
		}
	}

	public class DeleteNoticeToImproveResponse
	{
	}
}
