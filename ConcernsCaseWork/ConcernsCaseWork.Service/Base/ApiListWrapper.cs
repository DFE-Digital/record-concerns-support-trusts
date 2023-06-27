using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Base
{
	public sealed class ApiListWrapper<T>
	{
		[JsonProperty("data")] 
		public IList<T> Data { get; set; }
		
		[JsonProperty("paging")]
		public Pagination Paging { get; set; }

		public ApiListWrapper() { }

		[JsonConstructor]
		public ApiListWrapper(IList<T> data, Pagination paging) => (Data, Paging) = (data, paging);
	}
	public class Pagination
	{
		[JsonProperty("page")]
		public int Page { get; }

		[JsonProperty("recordCount")]
		public int RecordCount { get; }

		[JsonProperty("totalPages")]
		public int TotalPages { get; }

		[JsonProperty("nextPageUrl")]
		public string NextPageUrl { get; }

		[JsonProperty("hasPrevious")]
		public bool HasPrevious { get; }

		[JsonProperty("hasNext")]
		public bool HasNext { get; }

		[JsonConstructor]
		public Pagination(int page, int recordCount, int totalPages, string nextPageUrl, bool hasPrevious, bool hasNext) =>
			(Page, RecordCount, TotalPages, NextPageUrl, HasPrevious, HasNext) = (page, recordCount, totalPages, nextPageUrl, hasPrevious, hasNext);
	}
}