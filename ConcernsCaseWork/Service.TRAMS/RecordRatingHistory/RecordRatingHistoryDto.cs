using System;
using System.Text.Json.Serialization;

namespace Service.TRAMS.RecordRatingHistory
{
	public sealed class RecordRatingHistoryDto
	{
		[JsonPropertyName("created_at")]
		public DateTimeOffset CreatedAt { get; }

		[JsonPropertyName("record_urn")]
		public long RecordUrn { get; }
		
		[JsonPropertyName("rating_urn")]
		public long RatingUrn { get; }
		
		[JsonConstructor]
		public RecordRatingHistoryDto(DateTimeOffset createdAt, long recordUrn, long ratingUrn) => 
			(CreatedAt, RecordUrn, RatingUrn) = (createdAt, recordUrn, ratingUrn);
	}
}