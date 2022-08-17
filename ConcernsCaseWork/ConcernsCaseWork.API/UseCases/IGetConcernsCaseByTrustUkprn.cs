using ConcernsCaseWork.API.ResponseModels;
using System.Collections.Generic;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IGetConcernsCaseByTrustUkprn
    {
        public IList<ConcernsCaseResponse> Execute(string trustUkprn, int page, int count);
    }
}