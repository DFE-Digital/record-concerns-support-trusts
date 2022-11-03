using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Records
{
	[Serializable]
	public sealed class RecordDto
	{
		[JsonProperty("id")]
		public long Id { get; }
		
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
		
		[JsonProperty("typeId")]
		public long TypeId { get; }

		[JsonProperty("ratingId")]
		public long RatingId { get; }

		[JsonProperty("statusId")]
		public long StatusId { get; }
		
		[JsonConstructor]
		public RecordDto(DateTimeOffset createdAt, DateTimeOffset updatedAt, DateTimeOffset reviewAt, DateTimeOffset closedAt, 
			string name, string description, string reason, long caseUrn, long typeId, 
			long ratingId, long id, long statusId) => 
			(CreatedAt, UpdatedAt, ReviewAt, ClosedAt, Name, Description, Reason, CaseUrn, TypeId, RatingId, Id, StatusId) = 
			(createdAt, updatedAt, reviewAt, closedAt, name, description, reason, caseUrn, typeId, ratingId, id, statusId);
	}
}