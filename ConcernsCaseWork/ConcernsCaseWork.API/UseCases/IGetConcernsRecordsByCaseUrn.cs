using ConcernsCaseWork.API.ResponseModels;
using System.Collections.Generic;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IGetConcernsRecordsByCaseUrn
    {
        public IList<ConcernsRecordResponse> Execute(int caseUrn);
    }
}