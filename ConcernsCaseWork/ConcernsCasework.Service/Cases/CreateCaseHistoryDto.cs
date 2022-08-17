using Newtonsoft.Json;

namespace ConcernsCasework.Service.Cases
{
	public sealed class CreateCaseHistoryDto
	{
		[JsonProperty("createdAt")]
		public DateTimeOffset CreatedAt { get; }
		
		[JsonProperty("caseUrn")]
		public long CaseUrn { get; }
		
		[JsonProperty("action")]
		public string Action { get; }
		
		[JsonProperty("title")]
		public string Title { get; }
		
		[JsonProperty("description")]
		public string Description { get; }
		
		[JsonProperty("urn")]
		public long Urn { get; set; } // TODO Remove setter when Academy API is live
		
		[JsonConstructor]
		public CreateCaseHistoryDto(DateTimeOffset createdAt, long caseUrn, string action, string title, string description) => 
			(CreatedAt, CaseUrn, Action, Title, Description) = 
			(createdAt, caseUrn, action, title, description);
	}
}