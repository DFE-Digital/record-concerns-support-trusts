using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
{
    public interface IIndexConcernsStatuses
    {
        public IList<ConcernsStatusResponse> Execute();
    }
}