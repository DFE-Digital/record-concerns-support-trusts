using System;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Service.TRAMS.Dto
{
	public sealed class RecordDto
	{
		[JsonPropertyName("created_at")]
		public DateTimeOffset CreatedAt { get; }

		[JsonPropertyName("updated_at")]
		public DateTimeOffset UpdateAt { get; }
		
		[JsonPropertyName("review_at")]
		public DateTimeOffset ReviewAt { get; }
		
		[JsonPropertyName("closed_at")]
		public DateTimeOffset ClosedAt { get; }
		
		[JsonPropertyName("name")]
		public string Name { get; }
		
		[JsonPropertyName("description")]
		public string Description { get; }
		
		[JsonPropertyName("reason")]
		public string Reason { get; }
		
		[JsonPropertyName("case_urn")]
		public BigInteger CaseUrn { get; }
		
		[JsonPropertyName("type_urn")]
		public BigInteger TypeUrn { get; }

		[JsonPropertyName("rating_urn")]
		public BigInteger RatingUrn { get; }
		
		[JsonPropertyName("primary")]
		public bool Primary { get; }
		
		[JsonPropertyName("urn")]
		public BigInteger Urn { get; }
		
		[JsonPropertyName("status")]
		public string Status { get; }
		
		[JsonConstructor]
		public RecordDto(DateTimeOffset createdAt, DateTimeOffset updatedAt, DateTimeOffset reviewAt, 
			DateTimeOffset closedAt, string name, string description, string reason, BigInteger caseUrn, BigInteger typeUrn, 
			BigInteger ratingUrn, bool primary, BigInteger urn, string status) => 
			(CreatedAt, UpdateAt, ReviewAt, ClosedAt, Name, Description, Reason, CaseUrn, TypeUrn, RatingUrn, Primary, Urn, Status) = 
			(createdAt, updatedAt, reviewAt, closedAt, name, description, reason, caseUrn, typeUrn, ratingUrn, primary, urn, status);
	}
}