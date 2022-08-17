using ConcernsCaseWork.API.ResponseModels;
using System.Collections.Generic;

namespace ConcernsCaseWork.API.UseCases
{
    public interface IGetEstablishmentsByTrustUid
    {
        public List<EstablishmentResponse> Execute(string trustUid);
    }
}