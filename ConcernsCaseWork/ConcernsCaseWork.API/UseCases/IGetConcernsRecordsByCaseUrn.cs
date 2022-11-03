using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IGetConcernsRecordsByCaseUrn
    {
        public IList<ConcernsRecordResponse> Execute(int caseUrn);
    }
}