using ConcernsCaseWork.Service.Ratings;
using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Cases;

public record ActivePagedCaseSummaryDto()
{
	[JsonProperty("caseUrn")]
	public long CaseUrn { get; set; }
	[JsonProperty("createdBy")]
	public string CreatedBy { get; set; }
	[JsonProperty("createdAt")]
	public DateTime CreatedAt { get; set; }
	[JsonProperty("updatedAt")]
	public DateTime UpdatedAt { get; set; }
	[JsonProperty("statusName")]
	public string StatusName { get; set; }
	[JsonProperty("rating")]
	public RatingDto Rating { get; set; }
	[JsonProperty("trustUkPrn")]
	public string TrustUkPrn { get; set; }
	
	public record Paging
	{
		[JsonProperty("page")]
		public int Page { get; set; }
		[JsonProperty("recordCount")]
		public int RecordCount { get; set; }
		[JsonProperty("nextPageUrl")]
		public string NextPageUrl { get; set; }
		[JsonProperty("hasNext")]
		public bool HasNext { get; set; }
		[JsonProperty("hasPrevious")]
		public bool HasPrevious { get; set; }
		public string Sort { get; set; }
		public string SearchPhrase { get; set; }

		
	}
	
	
};

public record ActiveConcern
{
	
}


