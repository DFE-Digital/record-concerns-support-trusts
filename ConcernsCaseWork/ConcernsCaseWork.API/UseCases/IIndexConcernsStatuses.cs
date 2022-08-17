using ConcernsCaseWork.API.ResponseModels;
using System.Collections.Generic;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IIndexConcernsStatuses
    {
        public IList<ConcernsStatusResponse> Execute();
    }
}