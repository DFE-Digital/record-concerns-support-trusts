using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IUpdateConcernsCase
    {
       ConcernsCaseResponse Execute(int urn, ConcernCaseRequest request);
    }
}