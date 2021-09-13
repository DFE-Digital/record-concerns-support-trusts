using Newtonsoft.Json;
using System;

namespace Service.TRAMS.Records
{
	public sealed class UpdateRecordDto
	{
		[JsonProperty("updated_at")]
		public DateTimeOffset UpdateAt { get; }
		
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
		
		[JsonProperty("urn")]
		public long Urn { get; }
		
		[JsonProperty("status")]
		public long Status { get; }
		
		[JsonConstructor]
		public UpdateRecordDto(DateTimeOffset updatedAt, DateTimeOffset reviewAt, DateTimeOffset closedAt, 
			string name, string description, string reason, long caseUrn, long typeUrn, 
			long ratingUrn, bool primary, long urn, long status) => 
			(UpdateAt, ReviewAt, ClosedAt, Name, Description, Reason, CaseUrn, TypeUrn, RatingUrn, Primary, Urn, Status) = 
			(updatedAt, reviewAt, closedAt, name, description, reason, caseUrn, typeUrn, ratingUrn, primary, urn, status);
	}
}