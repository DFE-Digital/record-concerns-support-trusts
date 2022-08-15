using Concerns.Data.ResponseModels;

namespace Concerns.Data.UseCases
{
    public interface IGetEstablishmentsByTrustUid
    {
        public List<EstablishmentResponse> Execute(string trustUid);
    }
}