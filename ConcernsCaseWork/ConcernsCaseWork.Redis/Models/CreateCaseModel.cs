using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Redis.Models
{
	[Serializable]
	public sealed class CreateCaseModel
	{
		public DateTimeOffset CreatedAt { get; set; }

		public DateTimeOffset UpdatedAt { get; set; }

		public DateTimeOffset ReviewAt { get; set; }

		public DateTimeOffset ClosedAt { get; set; }

		public string CreatedBy { get; set; }
		
		public string CrmEnquiry { get; set; }
		
		public string TrustUkPrn { get; set; }
		
		public string ReasonAtReview { get; set; }
		
		public DateTimeOffset DeEscalation { get; set; }

		public string Issue { get; set; }

		public string CurrentStatus { get; set; }

		public string CaseAim { get; set; }
		
		public string DeEscalationPoint { get; set; }
		
		public string NextSteps { get; set; }
		
		public string DirectionOfTravel { get; set; }
		public string CaseHistory { get; set; }
		
		public long StatusId { get; set; }
		
		public long RatingId { get; set; }
		
		public string RagRatingName { get; set; }
		
		public Tuple<int, IList<string>> RagRating { get; set; }

		public IList<string> RagRatingCss { get; set; }

		public IList<CreateRecordModel> CreateRecordsModel { get; set; } = new List<CreateRecordModel>();
	}
}