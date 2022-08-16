using Concerns.Data.RequestModels;
using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
{
    public interface IUpdateConcernsRecord
    {
        ConcernsRecordResponse Execute(int urn, ConcernsRecordRequest request);
    }
}