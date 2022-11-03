using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.NTI.NoticeToImprove
{
    public class GetNoticeToImproveByCaseUrn : IUseCase<int, List<NoticeToImproveResponse>>
    {
        private readonly INoticeToImproveGateway _gateway;

        public GetNoticeToImproveByCaseUrn(INoticeToImproveGateway gateway)
        {
            _gateway = gateway;
        }

        public List<NoticeToImproveResponse> Execute(int caseUrn)
        {
            return ExecuteAsync(caseUrn).Result;
        }

        public async Task<List<NoticeToImproveResponse>> ExecuteAsync(int caseUrn)
        {
            var noticesToImprove = await _gateway.GetNoticeToImproveByCaseUrn(caseUrn);
            return noticesToImprove.Select(noticeToImprove => NoticeToImproveFactory.CreateResponse(noticeToImprove)).ToList();
        }
    }
}