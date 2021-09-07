using System;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Service.TRAMS.Models
{
	public sealed class RecordDto
	{
		[JsonPropertyName("id")]
		public int Id { get; }
		
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
		
		[JsonPropertyName("case_id")]
		public int CaseId { get; }
		
		[JsonPropertyName("type_id")]
		public int TypeId { get; }

		[JsonPropertyName("rating_id")]
		public int RatingId { get; }
		
		[JsonPropertyName("primary")]
		public bool Primary { get; }
		
		[JsonPropertyName("urn")]
		public BigInteger Urn { get; }
		
		[JsonPropertyName("status")]
		public int Status { get; }
		
		public RecordDto(int id, DateTimeOffset createdAt, DateTimeOffset updatedAt, DateTimeOffset reviewAt, 
			DateTimeOffset closedAt, string name, string description, string reason, int caseId, int typeId, 
			int ratingId, bool primary, BigInteger urn, int status) => 
			(Id, CreatedAt, UpdateAt, ReviewAt, ClosedAt, Name, Description, Reason, CaseId, TypeId, RatingId, Primary, Urn, Status) = 
			(id, createdAt, updatedAt, reviewAt, closedAt, name, description, reason, caseId, typeId, ratingId, primary, urn, status);
	}
}