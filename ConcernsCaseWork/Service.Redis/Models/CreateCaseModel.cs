using System;
using System.Collections.Generic;

namespace Service.Redis.Models
{
	[Serializable]
	public sealed class CreateCaseModel
	{
		public DateTimeOffset CreatedAt { get; set; }

		public DateTimeOffset UpdatedAt { get; set; }

		public DateTimeOffset ReviewAt { get; set; }

		public DateTimeOffset ClosedAt { get; set; }

		public string CreatedBy { get; set; }

		public string Description { get; set; }
		
		public string CrmEnquiry { get; set; }
		
		public string TrustName { get; set; }
		
		public string TrustUkPrn { get; set; }
		
		public string ReasonAtReview { get; set; }
		
		public DateTimeOffset DeEscalation { get; set; }

		public string Issue { get; set; }

		public string CurrentStatus { get; set; }

		public string CaseAim { get; set; }
		
		public string DeEscalationPoint { get; set; }
		
		public string NextSteps { get; set; }
		
		public string DirectionOfTravel { get; set; }
		
		public long StatusUrn { get; set; }
		
		public string TypeDisplay
		{
			get
			{
				var separator = string.IsNullOrEmpty(SubType) ? string.Empty : ":";
				return $"{Type}{separator} {SubType ?? string.Empty}";
			}
		}
		
		public long TypeUrn { get; set; }
		
		public string Type { get; set; }
		
		public string SubType { get; set; }
		
		public string RagRatingName { get; set; }
		
		public Tuple<int, IList<string>> RagRating { get; set; }

		public IList<string> RagRatingCss { get; set; }
	}
}