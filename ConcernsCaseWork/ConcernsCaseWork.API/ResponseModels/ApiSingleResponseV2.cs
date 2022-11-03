namespace ConcernsCaseWork.API.ResponseModels
{
    public class ApiSingleResponseV2<TResponse> where TResponse : class
    {
        public TResponse Data { get; set; }
        
        public ApiSingleResponseV2() => Data = null;
        public ApiSingleResponseV2(TResponse data) => Data = data ;
    }
}