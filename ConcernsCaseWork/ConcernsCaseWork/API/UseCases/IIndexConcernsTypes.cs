using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
{
    public interface IIndexConcernsTypes
    {
        public IList<ConcernsTypeResponse> Execute();
    }
}