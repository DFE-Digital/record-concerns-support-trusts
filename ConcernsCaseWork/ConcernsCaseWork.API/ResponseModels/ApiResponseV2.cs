namespace ConcernsCaseWork.API.ResponseModels
{
    public class ApiResponseV2<TResponse> where TResponse : class
    {
        public IEnumerable<TResponse> Data { get; set; }
        public PagingResponse Paging { get; set; }
        
        public ApiResponseV2() => Data = new List<TResponse>();

        public ApiResponseV2(IEnumerable<TResponse> data, PagingResponse pagingResponse)
        {
            Data = data;
            Paging = pagingResponse;
        } 
        
        public ApiResponseV2(TResponse data) => Data = new List<TResponse>{ data };
    }
}