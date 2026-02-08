using ConcernsCaseWork.API.Contracts.Case;
using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Cases
{
	public sealed class CreateCaseDto
	{
		[JsonProperty("createdAt")]
		public DateTimeOffset CreatedAt { get; }

		[JsonProperty("updatedAt")]
		public DateTimeOffset UpdatedAt { get; }
		
		[JsonProperty("reviewAt")]
		public DateTimeOffset ReviewAt { get; }

		[JsonProperty("createdBy")]
		public string CreatedBy { get; }
		
		[JsonProperty("crmEnquiry")]
		public string CrmEnquiry { get; }
		
		[JsonProperty("trustUkprn")]
		public string TrustUkPrn { get; }
		
		[JsonProperty("trustCompaniesHouseNumber")]
		public string TrustCompaniesHouseNumber { get;  }

		[JsonProperty("reasonAtReview")]
		public string ReasonAtReview { get; }

		[JsonProperty("deEscalation")]
		public DateTimeOffset DeEscalation { get; }
		
		[JsonProperty("issue")]
		public string Issue { get; }

		[JsonProperty("currentStatus")]
		public string CurrentStatus { get; }

		[JsonProperty("caseAim")]
		public string CaseAim { get; }
		
		[JsonProperty("deEscalationPoint")]
		public string DeEscalationPoint { get; }
		
		[JsonProperty("nextSteps")]
		public string NextSteps { get; }
		
		[JsonProperty("caseHistory")]
		public string CaseHistory { get; set; }
		
		[JsonProperty("territory")]
		public Territory? Territory { get; set; }

		[JsonProperty("division")]
		public Division? Division { get; set; }

		[JsonProperty("region")]
		public Region? Region { get; set; }

		/// <summary>
		/// Deteriorating, unchanged, improved
		/// </summary>
		[JsonProperty("directionOfTravel")]
		public string DirectionOfTravel { get; }
		
		[JsonProperty("statusId")]
		public long StatusId { get; }
		
		[JsonProperty("ratingId")]
		public long RatingId { get; }

		[JsonProperty("ratingRational")]
		public bool RatingRational { get; }

		[JsonProperty("ratingRationalCommentary")]
		public string RatingRationalCommentary { get; }

		[JsonConstructor]
		public CreateCaseDto(DateTimeOffset createdAt, DateTimeOffset updatedAt, DateTimeOffset reviewAt, 
			string createdBy, string crmEnquiry, string trustUkPrn, 
			string reasonAtReview, DateTimeOffset deEscalation, string issue, string currentStatus, 
			string nextSteps, string caseAim, string deEscalationPoint, string caseHistory, string directionOfTravel, long statusId,
			long ratingId, bool ratingRational, string ratingRationalCommentary, Territory? territory, string trustCompaniesHouseNumber, Division? division, Region? region) => 
			(CreatedAt, UpdatedAt, ReviewAt, CreatedBy, CrmEnquiry, TrustUkPrn,
				ReasonAtReview, DeEscalation, Issue, CurrentStatus, NextSteps, CaseAim, DeEscalationPoint, CaseHistory, DirectionOfTravel, 
				StatusId, RatingId, RatingRational, RatingRationalCommentary, Territory, TrustCompaniesHouseNumber, Division, Region) = 
			(createdAt, updatedAt, reviewAt, createdBy, crmEnquiry, trustUkPrn,
				reasonAtReview, deEscalation, issue, currentStatus, nextSteps, caseAim, deEscalationPoint, caseHistory, directionOfTravel, 
				statusId, ratingId, ratingRational, ratingRationalCommentary, territory, trustCompaniesHouseNumber, division, region);
	}
}