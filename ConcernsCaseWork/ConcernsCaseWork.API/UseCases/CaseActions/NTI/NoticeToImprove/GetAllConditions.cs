using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.UseCases.CaseActions.NTI.NoticeToImprove
{
    public class GetAllConditions : IUseCase<Object, List<NoticeToImproveCondition>>
    {
        private readonly INoticeToImproveGateway _gateway;

        public GetAllConditions(INoticeToImproveGateway gateway)
        {
            _gateway = gateway;
        }

        public List<NoticeToImproveCondition> Execute(Object ignore)
        {
            return ExecuteAsync(ignore).Result;
        }

        public async Task<List<NoticeToImproveCondition>> ExecuteAsync(Object ignore)
        {
            return await _gateway.GetAllConditions();
        }
    }
}
