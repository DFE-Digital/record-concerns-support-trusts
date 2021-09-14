using Newtonsoft.Json;
using System;

namespace Service.TRAMS.RecordRatingHistory
{
	public sealed class RecordRatingHistoryDto
	{
		[JsonProperty("created_at")]
		public DateTimeOffset CreatedAt { get; }

		[JsonProperty("record_urn")]
		public long RecordUrn { get; }
		
		[JsonProperty("rating_urn")]
		public long RatingUrn { get; }
		
		[JsonConstructor]
		public RecordRatingHistoryDto(DateTimeOffset createdAt, long recordUrn, long ratingUrn) => 
			(CreatedAt, RecordUrn, RatingUrn) = (createdAt, recordUrn, ratingUrn);
	}
}