using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
{
    public interface IGetConcernsCasesByOwnerId
    {
        IList<ConcernsCaseResponse> Execute(string ownerId, int? statusUrn, int page, int count);
    }
}