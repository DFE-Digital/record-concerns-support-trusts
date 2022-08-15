namespace Concerns.Data.ResponseModels
{
    public class AddressResponse
    {
        public string Street { get; set; }
        public string Locality { get; set; }
        public string AdditionalLine { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
    }
}