using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IIndexConcernsTypes
    {
        public IList<ConcernsTypeResponse> Execute();
    }
}