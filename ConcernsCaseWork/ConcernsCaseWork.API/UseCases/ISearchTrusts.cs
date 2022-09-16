using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.UseCases
{
    public interface ISearchTrusts
    {
        public IEnumerable<TrustSummaryResponse> Execute(int page, int count, string groupName, string urn, string companiesHouseNumber);
    }
}