using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
{
    public interface IGetConcernsRecordsByCaseUrn
    {
        public IList<ConcernsRecordResponse> Execute(int caseUrn);
    }
}