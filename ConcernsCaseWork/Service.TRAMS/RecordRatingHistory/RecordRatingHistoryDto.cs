using Newtonsoft.Json;
using System;

namespace Service.TRAMS.RecordRatingHistory
{
	[Serializable]
	public sealed class RecordRatingHistoryDto
	{
		[JsonProperty("createdAt")]
		public DateTimeOffset CreatedAt { get; }

		[JsonProperty("recordUrn")]
		public long RecordUrn { get; }
		
		[JsonProperty("ratingId")]
		public long RatingId { get; }
		
		[JsonConstructor]
		public RecordRatingHistoryDto(DateTimeOffset createdAt, long recordUrn, long ratingId) => 
			(CreatedAt, RecordUrn, RatingId) = (createdAt, recordUrn, ratingId);
	}
}