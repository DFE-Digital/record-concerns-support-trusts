namespace ConcernsCaseWork.API.ResponseModels
{
    public class TrustSummaryResponse
    {
        public string Ukprn { get; set; }
        public string Urn { get; set; }
        public string GroupName { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public string TrustType { get; set; }
        public AddressResponse TrustAddress { get; set; }
        public List<EstablishmentSummaryResponse> Establishments { get; set; }
    }
}