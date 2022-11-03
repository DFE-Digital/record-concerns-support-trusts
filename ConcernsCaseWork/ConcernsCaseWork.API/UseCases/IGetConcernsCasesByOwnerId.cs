using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IGetConcernsCasesByOwnerId
    {
        IList<ConcernsCaseResponse> Execute(string ownerId, int? statusId, int page, int count);
    }
}