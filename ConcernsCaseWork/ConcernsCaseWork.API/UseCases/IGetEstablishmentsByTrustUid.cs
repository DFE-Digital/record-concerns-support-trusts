using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IGetEstablishmentsByTrustUid
    {
        public List<EstablishmentResponse> Execute(string trustUid);
    }
}