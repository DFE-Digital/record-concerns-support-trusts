using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
{
    public interface IGetConcernsCaseByTrustUkprn
    {
        public IList<ConcernsCaseResponse> Execute(string trustUkprn, int page, int count);
    }
}