using System;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Service.TRAMS.Records
{
	public sealed class UpdateRecordDto
	{
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
		public BigInteger Status { get; }
		
		[JsonConstructor]
		public UpdateRecordDto(DateTimeOffset updatedAt, DateTimeOffset reviewAt, DateTimeOffset closedAt, 
			string name, string description, string reason, BigInteger caseUrn, BigInteger typeUrn, 
			BigInteger ratingUrn, bool primary, BigInteger urn, BigInteger status) => 
			(UpdateAt, ReviewAt, ClosedAt, Name, Description, Reason, CaseUrn, TypeUrn, RatingUrn, Primary, Urn, Status) = 
			(updatedAt, reviewAt, closedAt, name, description, reason, caseUrn, typeUrn, ratingUrn, primary, urn, status);
	}
}