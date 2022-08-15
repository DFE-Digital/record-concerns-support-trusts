using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
{
    public interface IGetTrustByUkprn
    {
        public TrustResponse Execute(string ukprn);
    }
}