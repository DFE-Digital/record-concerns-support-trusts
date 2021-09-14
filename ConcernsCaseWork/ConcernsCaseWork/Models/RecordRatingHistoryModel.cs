using System;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class RecordRatingHistoryModel
	{
		public DateTimeOffset CreatedAt { get; }
		
		public long RecordUrn { get; }
		
		public long RatingUrn { get; }
		
		public RecordRatingHistoryModel(DateTimeOffset createdAt, long recordUrn, long ratingUrn) => 
			(CreatedAt, RecordUrn, RatingUrn) = (createdAt, recordUrn, ratingUrn);
	}
}