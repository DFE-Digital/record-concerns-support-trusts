using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
    public interface ICreateConcernsCase
    {
        public ConcernsCaseResponse Execute(ConcernCaseRequest request);
    }
}