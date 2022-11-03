using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IIndexConcernsStatuses
    {
        public IList<ConcernsStatusResponse> Execute();
    }
}