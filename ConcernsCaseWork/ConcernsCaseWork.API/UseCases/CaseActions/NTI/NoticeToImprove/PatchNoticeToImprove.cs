using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.NTI.NoticeToImprove
{
    public class PatchNoticeToImprove : IUseCase<PatchNoticeToImproveRequest, NoticeToImproveResponse>
    {
        private readonly INoticeToImproveGateway _gateway;

        public PatchNoticeToImprove(INoticeToImproveGateway gateway)
        {
            _gateway = gateway;
        }

        public NoticeToImproveResponse Execute(PatchNoticeToImproveRequest request)
        {
            return ExecuteAsync(request).Result;
        }

        public async Task<NoticeToImproveResponse> ExecuteAsync(PatchNoticeToImproveRequest request)
        {
            var patchedNoticeToImprove = await _gateway.PatchNoticeToImprove(NoticeToImproveFactory.CreateDBModel(request));
            return NoticeToImproveFactory.CreateResponse(patchedNoticeToImprove);
        }
    }
}
