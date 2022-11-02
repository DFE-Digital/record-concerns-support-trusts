using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
    public interface ICreateConcernsRecord
    {
        public ConcernsRecordResponse Execute(ConcernsRecordRequest request);
    }
}