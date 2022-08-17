using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IGetTrustByUkprn
    {
        public TrustResponse Execute(string ukprn);
    }
}