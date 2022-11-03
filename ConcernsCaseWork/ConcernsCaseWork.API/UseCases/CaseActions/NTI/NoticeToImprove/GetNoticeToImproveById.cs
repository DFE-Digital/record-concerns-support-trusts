using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.NTI.NoticeToImprove
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
            return NoticeToImproveFactory.CreateResponse(noticeToImprove);
        }
    }
}
