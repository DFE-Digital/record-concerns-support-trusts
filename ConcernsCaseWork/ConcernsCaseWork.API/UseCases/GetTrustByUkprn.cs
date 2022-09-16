using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
    public class GetTrustByUkprn : IGetTrustByUkprn
    {
        private readonly ITrustGateway _trustGateway;
        private readonly IGetEstablishmentsByTrustUid _getEstablishmentsByTrustUid;

        public GetTrustByUkprn(ITrustGateway trustGateway, IGetEstablishmentsByTrustUid getEstablishmentsByTrustUid)
        {
            _trustGateway = trustGateway;
            _getEstablishmentsByTrustUid = getEstablishmentsByTrustUid;
        }
        
        public TrustResponse Execute(string ukprn)
        {
            var group = _trustGateway.GetGroupByUkPrn(ukprn);
            if (group == null)
            {
                return null;
            }

            var trust = _trustGateway.GetIfdTrustByGroupId(group.GroupId);
            var establishments = _getEstablishmentsByTrustUid.Execute(group.GroupUid);
            return TrustResponseFactory.Create(group, trust, establishments);
        }
    }
}