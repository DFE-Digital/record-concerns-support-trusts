using Concerns.Data.RequestModels;
using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
{
    public interface IUpdateConcernsCase
    {
       ConcernsCaseResponse Execute(int urn, ConcernCaseRequest request);
    }
}