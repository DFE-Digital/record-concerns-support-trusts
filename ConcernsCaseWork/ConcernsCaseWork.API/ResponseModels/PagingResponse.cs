namespace ConcernsCaseWork.API.ResponseModels
{
    public class PagingResponse
    {
        public int Page { get; set; }
        public int RecordCount { get; set; }
        public string NextPageUrl { get; set; }
    }
}