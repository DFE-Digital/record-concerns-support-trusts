using System;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class RecordRatingHistoryModel
	{
		public int Id { get; }
		
		public DateTimeOffset CreatedAt { get; }
		
		public int RecordId { get; }
		
		public int RatingId { get; }
		
		public RecordRatingHistoryModel(int id, DateTimeOffset createdAt, int recordId, int ratingId) => 
			(Id, CreatedAt, RecordId, RatingId) = (id, createdAt, recordId, ratingId);
	}
}