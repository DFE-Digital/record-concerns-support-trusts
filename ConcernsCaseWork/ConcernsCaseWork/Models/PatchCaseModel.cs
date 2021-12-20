using System;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class PatchCaseModel
	{
		public DateTimeOffset UpdatedAt { get; set; }
		
		public DateTimeOffset? ReviewAt { get; set; }
		
		public DateTimeOffset? ClosedAt { get; set; }
		
		public string CreatedBy { get; set; }
		
		public long Urn { get; set; }
		
		public long RatingUrn { get; set; }
		
		public string ReasonAtReview { get; set; }
		
		public string StatusName { get; set; }
		
		public string DirectionOfTravel { get; set; }
		
		public string Issue { get; set; }
		
		public string CurrentStatus { get; set; }
		
		public string CaseAim { get; set; }
		
		public string DeEscalationPoint { get; set; }
		
		public string NextSteps { get; set; }
	}
}