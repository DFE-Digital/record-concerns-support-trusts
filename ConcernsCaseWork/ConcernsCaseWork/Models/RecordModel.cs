using System;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class RecordModel
	{
		public DateTimeOffset CreatedAt { get; }

		public DateTimeOffset UpdatedAt { get; }

		public DateTimeOffset ReviewAt { get; }

		public DateTimeOffset ClosedAt { get; }

		public string Name { get; }
	
		public string Description { get; }

		public string Reason { get; }
	
		public long CaseUrn { get; }

		public long TypeUrn { get; }

		public long RatingUrn { get; }
		
		public long Urn { get; }
		
		public long StatusUrn { get; }
		
		public RecordModel(DateTimeOffset createdAt, DateTimeOffset updatedAt, DateTimeOffset reviewAt, 
			DateTimeOffset closedAt, string name, string description, string reason, long caseUrn, long typeUrn, 
			long ratingUrn, long urn, long statusUrn) => 
			(CreatedAt, UpdatedAt, ReviewAt, ClosedAt, Name, Description, Reason, CaseUrn, TypeUrn, RatingUrn, Urn, StatusUrn) = 
			(createdAt, updatedAt, reviewAt, closedAt, name, description, reason, caseUrn, typeUrn, ratingUrn, urn, statusUrn);
	}
}