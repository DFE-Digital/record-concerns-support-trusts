using System;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class PatchRecordModel
	{
		public DateTimeOffset UpdatedAt { get; set; }
		public DateTimeOffset? ClosedAt { get; set; }
		public long Urn { get; set; }
		public long CaseUrn { get; set; }
		public long RatingUrn { get; set; }
		public string CreatedBy { get; set; }
		public long StatusUrn { get; set; }

	}
}