using System;

namespace ConcernsCaseWork.Models
{
	public sealed class PatchCaseModel
	{
		public DateTimeOffset UpdatedAt { get; set; }
		
		public DateTimeOffset? ReviewAt { get; set; }
		
		public DateTimeOffset? ClosedAt { get; set; }
		
		public string CreatedBy { get; set; }
		
		public long Urn { get; set; }
		
		public string CaseType { get; set; }
		
		public string CaseSubType { get; set; }
		
		public long TypeUrn { get; set; }
		
		public string RiskRating { get; set; }
		
		public long RatingUrn { get; set; }
		
		public string ReasonAtReview { get; set; }
		
		public string StatusName { get; set; }
		
		public string DirectionOfTravel { get; set; }
		
		public string Issue { get; set; }
		
		public string CurrentStatus { get; set; }
		
		public string CaseAim { get; set; }
	}
}