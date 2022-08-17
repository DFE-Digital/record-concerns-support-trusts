using Concerns.Data.Gateways;
using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.API.UseCases
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