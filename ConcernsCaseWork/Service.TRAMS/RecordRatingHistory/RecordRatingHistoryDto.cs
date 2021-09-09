using System;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Service.TRAMS.RecordRatingHistory
{
	public sealed class RecordRatingHistoryDto
	{
		[JsonPropertyName("created_at")]
		public DateTimeOffset CreatedAt { get; }

		[JsonPropertyName("record_urn")]
		public BigInteger RecordUrn { get; }
		
		[JsonPropertyName("rating_name")]
		public string RatingName { get; }
		
		[JsonPropertyName("rating_urn")]
		public BigInteger RatingUrn { get; }
		
		[JsonConstructor]
		public RecordRatingHistoryDto(DateTimeOffset createdAt, BigInteger recordUrn, string ratingName, BigInteger ratingUrn) => 
			(CreatedAt, RecordUrn, RatingName, RatingUrn) = (createdAt, recordUrn, ratingName, ratingUrn);
	}
}