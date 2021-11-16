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
		
		[JsonProperty("ratingUrn")]
		public long RatingUrn { get; }
		
		[JsonConstructor]
		public RecordRatingHistoryDto(DateTimeOffset createdAt, long recordUrn, long ratingUrn) => 
			(CreatedAt, RecordUrn, RatingUrn) = (createdAt, recordUrn, ratingUrn);
	}
}