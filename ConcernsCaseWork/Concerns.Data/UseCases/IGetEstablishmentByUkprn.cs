using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
{
    public interface IGetEstablishmentByUkprn
    {
        public EstablishmentResponse Execute(string ukprn);
    }
}