using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Records
{
	[Serializable]
	public sealed class RecordDto
	{
		[JsonProperty("createdAt")]
		public DateTimeOffset CreatedAt { get; }

		[JsonProperty("updatedAt")]
		public DateTimeOffset UpdatedAt { get; }
		
		[JsonProperty("reviewAt")]
		public DateTimeOffset ReviewAt { get; }
		
		[JsonProperty("closedAt")]
		public DateTimeOffset ClosedAt { get; }
		
		[JsonProperty("name")]
		public string Name { get; }
		
		[JsonProperty("description")]
		public string Description { get; }
		
		[JsonProperty("reason")]
		public string Reason { get; }
		
		[JsonProperty("caseUrn")]
		public long CaseUrn { get; }
		
		[JsonProperty("typeUrn")]
		public long TypeUrn { get; }

		[JsonProperty("ratingUrn")]
		public long RatingUrn { get; }
		
		[JsonProperty("urn")]
		public long Urn { get; set; }
		
		[JsonProperty("statusUrn")]
		public long StatusUrn { get; }
		
		[JsonConstructor]
		public RecordDto(DateTimeOffset createdAt, DateTimeOffset updatedAt, DateTimeOffset reviewAt, DateTimeOffset closedAt, 
			string name, string description, string reason, long caseUrn, long typeUrn, 
			long ratingUrn, long urn, long statusUrn) => 
			(CreatedAt, UpdatedAt, ReviewAt, ClosedAt, Name, Description, Reason, CaseUrn, TypeUrn, RatingUrn, Urn, StatusUrn) = 
			(createdAt, updatedAt, reviewAt, closedAt, name, description, reason, caseUrn, typeUrn, ratingUrn, urn, statusUrn);
	}
}