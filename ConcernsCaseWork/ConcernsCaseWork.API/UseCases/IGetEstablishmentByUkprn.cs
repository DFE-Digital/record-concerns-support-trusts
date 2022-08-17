using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IGetEstablishmentByUkprn
    {
        public EstablishmentResponse Execute(string ukprn);
    }
}