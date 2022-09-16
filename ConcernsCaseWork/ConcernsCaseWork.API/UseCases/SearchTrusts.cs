using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
    public class SearchTrusts : ISearchTrusts
    {
        private readonly ITrustGateway _trustGateway;
        private readonly IEstablishmentGateway _establishmentGateway;

        public SearchTrusts(ITrustGateway trustGateway, IEstablishmentGateway establishmentGateway)
        {
            _trustGateway = trustGateway;
            _establishmentGateway = establishmentGateway;
        }
        
        public IEnumerable<TrustSummaryResponse> Execute(int page, int count, string groupName, string ukPrn, string companiesHouseNumber)
        {
            var groups = _trustGateway.SearchGroups(page, count, groupName, ukPrn, companiesHouseNumber);

            return groups.Select(group =>
            {
                var trust = _trustGateway.GetIfdTrustByGroupId(group.GroupId);
                var establishments = _establishmentGateway.GetByTrustUid(group.GroupUid);
                return TrustSummaryResponseFactory.Create(group, establishments, trust);
            });
        }
    }
}