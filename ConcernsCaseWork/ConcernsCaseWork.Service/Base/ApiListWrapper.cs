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
			
			[JsonConstructor]
			public Pagination(int page, int recordCount, string nextPageUrl) => 
				(Page, RecordCount, NextPageUrl) = (page, recordCount, nextPageUrl);
		}
	}
}