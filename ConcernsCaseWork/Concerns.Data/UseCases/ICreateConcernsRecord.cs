using Concerns.Data.RequestModels;
using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
{
    public interface ICreateConcernsRecord
    {
        public ConcernsRecordResponse Execute(ConcernsRecordRequest request);
    }
}