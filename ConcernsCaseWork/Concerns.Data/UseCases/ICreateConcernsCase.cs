using Concerns.Data.RequestModels;
using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
{
    public interface ICreateConcernsCase
    {
        public ConcernsCaseResponse Execute(ConcernCaseRequest request);
    }
}