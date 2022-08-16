using Concerns.Data.Factories;
using Concerns.Data.Gateways;
using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
{
    public class GetConcernsRecordsByCaseUrn : IGetConcernsRecordsByCaseUrn
    {
        private readonly IConcernsCaseGateway _concernsCaseGateway;

        public GetConcernsRecordsByCaseUrn(
            IConcernsCaseGateway concernsCaseGateway)
        {
            _concernsCaseGateway = concernsCaseGateway;
        }
        public IList<ConcernsRecordResponse> Execute(int caseUrn)
        {
            var concernsCase = _concernsCaseGateway.GetConcernsCaseIncludingRecordsByUrn(caseUrn);
            return concernsCase?.ConcernsRecords
                .Select(ConcernsRecordResponseFactory.Create).ToList();
        }
    }
}