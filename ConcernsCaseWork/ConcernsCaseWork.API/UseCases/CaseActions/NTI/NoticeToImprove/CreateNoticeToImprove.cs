using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.NTI.NoticeToImprove
{
    public class CreateNoticeToImprove : IUseCase<CreateNoticeToImproveRequest, NoticeToImproveResponse>
    {
        private readonly INoticeToImproveGateway _gateway;

        public CreateNoticeToImprove(INoticeToImproveGateway gateway)
        {
            _gateway = gateway;
        }

        public NoticeToImproveResponse Execute(CreateNoticeToImproveRequest request)
        {
            return ExecuteAsync(request).Result;
        }

        public async Task<NoticeToImproveResponse> ExecuteAsync(CreateNoticeToImproveRequest request)
        {
            var dbModel = NoticeToImproveFactory.CreateDBModel(request);

            var createdNoticeToImprove = await _gateway.CreateNoticeToImprove(dbModel);

            return NoticeToImproveFactory.CreateResponse(createdNoticeToImprove);
        }
    }
}
