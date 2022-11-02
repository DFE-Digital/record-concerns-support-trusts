using ConcernsCaseWork.API.RequestModels;
using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IUpdateConcernsRecord
    {
        ConcernsRecordResponse Execute(int urn, ConcernsRecordRequest request);
    }
}