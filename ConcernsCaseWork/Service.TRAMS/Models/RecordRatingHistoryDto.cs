using System;
using System.Text.Json.Serialization;

namespace Service.TRAMS.Models
{
	public sealed class RecordRatingHistoryDto
	{
		[JsonPropertyName("id")]
		public int Id { get; }
		
		[JsonPropertyName("created_at")]
		public DateTimeOffset CreatedAt { get; }

		[JsonPropertyName("record_id")]
		public int RecordId { get; }
		
		[JsonPropertyName("rating_id")]
		public int RatingId { get; }
		
		public RecordRatingHistoryDto(int id, DateTimeOffset createdAt, int recordId, int ratingId) => 
			(Id, CreatedAt, RecordId, RatingId) = (id, createdAt, recordId, ratingId);
	}
}