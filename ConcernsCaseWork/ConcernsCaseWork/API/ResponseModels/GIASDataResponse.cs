namespace Concerns.Data.ResponseModels
{
    public class GIASDataResponse
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupType { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public AddressResponse GroupContactAddress { get; set; }
        public string Ukprn { get; set; }
    }
}