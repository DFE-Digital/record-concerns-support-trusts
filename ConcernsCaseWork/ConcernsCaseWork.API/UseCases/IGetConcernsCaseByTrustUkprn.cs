using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IGetConcernsCaseByTrustUkprn
    {
        public IList<ConcernsCaseResponse> Execute(string trustUkprn, int page, int count);
    }
}