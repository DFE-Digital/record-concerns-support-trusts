using ConcernsCaseWork.API.ResponseModels;
using System.Collections.Generic;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IGetConcernsCasesByOwnerId
    {
        IList<ConcernsCaseResponse> Execute(string ownerId, int? statusUrn, int page, int count);
    }
}