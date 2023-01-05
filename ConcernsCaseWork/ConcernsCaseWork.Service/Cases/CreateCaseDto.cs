using ConcernsCaseWork.API.Contracts.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
		
		/// <summary>
		/// Deteriorating, unchanged, improved
		/// </summary>
		[JsonProperty("directionOfTravel")]
		public string DirectionOfTravel { get; }
		
		[JsonProperty("statusId")]
		public long StatusId { get; }
		
		[JsonProperty("ratingId")]
		public long RatingId { get; }

		[JsonConstructor]
		public CreateCaseDto(DateTimeOffset createdAt, DateTimeOffset updatedAt, DateTimeOffset reviewAt, 
			string createdBy, string crmEnquiry, string trustUkPrn, 
			string reasonAtReview, DateTimeOffset deEscalation, string issue, string currentStatus, 
			string nextSteps, string caseAim, string deEscalationPoint, string caseHistory, string directionOfTravel, long statusId,
			long ratingId, Territory? territory) => 
			(CreatedAt, UpdatedAt, ReviewAt, CreatedBy, CrmEnquiry, TrustUkPrn,
				ReasonAtReview, DeEscalation, Issue, CurrentStatus, NextSteps, CaseAim, DeEscalationPoint, CaseHistory, DirectionOfTravel, 
				StatusId, RatingId, Territory) = 
			(createdAt, updatedAt, reviewAt, createdBy, crmEnquiry, trustUkPrn,
				reasonAtReview, deEscalation, issue, currentStatus, nextSteps, caseAim, deEscalationPoint, caseHistory, directionOfTravel, 
				statusId, ratingId, territory);
	}
}