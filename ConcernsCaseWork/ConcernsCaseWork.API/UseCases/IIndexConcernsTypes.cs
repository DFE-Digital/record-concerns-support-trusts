using ConcernsCaseWork.API.ResponseModels;
using System.Collections.Generic;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IIndexConcernsTypes
    {
        public IList<ConcernsTypeResponse> Execute();
    }
}