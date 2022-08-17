namespace ConcernsCaseWork.API.RequestModels
{
    public class SearchEstablishmentsRequest
    {
        public int? Urn { get; set; }
        public string Ukprn { get; set; }
        public string Name { get; set; }
    }
}