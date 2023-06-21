using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Base
{
	public sealed class ApiListWrapper<T>
	{
		[JsonProperty("data")] 
		public IList<T> Data { get; }
		
		[JsonProperty("paging")]
		public Pagination Paging { get; }

		[JsonConstructor]
		public ApiListWrapper(IList<T> data, Pagination paging) => (Data, Paging) = (data, paging);

		public class Pagination
		{
			[JsonProperty("page")]
			public int Page { get; }
			
			[JsonProperty("recordCount")]
			public int RecordCount { get; }
			
			[JsonProperty("nextPageUrl")]
			public string NextPageUrl { get; }
			
			
			[JsonProperty("hasNext")]
			public bool HasNext { get; }
			
			
			[JsonProperty("hasPrevious")]
			public bool HasPrevious { get; }
			
			
			[JsonConstructor]
			public Pagination(int page, int recordCount, string nextPageUrl, bool hasNext, bool hasPrevious) => 
				(Page, RecordCount, NextPageUrl, HasNext, HasPrevious) = (page, recordCount, nextPageUrl, hasNext, hasPrevious);
		}
	}
}