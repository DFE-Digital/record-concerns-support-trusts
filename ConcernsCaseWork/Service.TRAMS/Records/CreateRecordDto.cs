using Newtonsoft.Json;
using System;

namespace Service.TRAMS.Records
{
	public sealed class CreateRecordDto
	{
		[JsonProperty("created_at")]
		public DateTimeOffset CreatedAt { get; }

		[JsonProperty("updated_at")]
		public DateTimeOffset UpdatedAt { get; }
		
		[JsonProperty("review_at")]
		public DateTimeOffset ReviewAt { get; }
		
		[JsonProperty("closed_at")]
		public DateTimeOffset ClosedAt { get; }
		
		[JsonProperty("name")]
		public string Name { get; }
		
		[JsonProperty("description")]
		public string Description { get; }
		
		[JsonProperty("reason")]
		public string Reason { get; }
		
		[JsonProperty("case_urn")]
		public long CaseUrn { get; }
		
		[JsonProperty("type_urn")]
		public long TypeUrn { get; }

		[JsonProperty("rating_urn")]
		public long RatingUrn { get; }
		
		[JsonProperty("primary")]
		public bool Primary { get; }
		
		[JsonProperty("status")]
		public long Status { get; }
		
		[JsonConstructor]
		public CreateRecordDto(DateTimeOffset createdAt, DateTimeOffset updatedAt, DateTimeOffset reviewAt, DateTimeOffset closedAt, 
			string name, string description, string reason, long caseUrn, long typeUrn, 
			long ratingUrn, bool primary, long status) => 
			(CreatedAt, UpdatedAt, ReviewAt, ClosedAt, Name, Description, Reason, CaseUrn, TypeUrn, RatingUrn, Primary, Status) = 
			(createdAt, updatedAt, reviewAt, closedAt, name, description, reason, caseUrn, typeUrn, ratingUrn, primary, status);
	}
}