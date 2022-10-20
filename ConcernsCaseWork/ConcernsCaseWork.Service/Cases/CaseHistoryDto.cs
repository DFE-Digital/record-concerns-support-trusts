using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Cases
{
	[Serializable]
	public sealed class CaseHistoryDto
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
		public long Urn { get; }
		
		[JsonConstructor]
		public CaseHistoryDto(DateTimeOffset createdAt, long caseUrn, string action, string title, string description, long urn) => 
			(CreatedAt, CaseUrn, Action, Title, Description, Urn) = 
			(createdAt, caseUrn, action, title, description, urn);
	}
}