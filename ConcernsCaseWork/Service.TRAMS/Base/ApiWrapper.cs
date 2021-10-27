using Newtonsoft.Json;
using System.Collections.Generic;

namespace Service.TRAMS.Base
{
	public class ApiWrapper<T>
	{
		[JsonProperty("data")] 
		public IEnumerable<T> Data { get; }
		
		[JsonProperty("paging")]
		public Pagination Paging { get; }

		[JsonConstructor]
		public ApiWrapper(IEnumerable<T> data, Pagination paging) => (Data, Paging) = (data, paging);

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