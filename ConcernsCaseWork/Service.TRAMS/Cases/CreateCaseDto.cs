using Newtonsoft.Json;
using System;

namespace Service.TRAMS.Cases
{
	public sealed class CreateCaseDto
	{
		[JsonProperty("created_at")]
		public DateTimeOffset CreatedAt { get; }

		[JsonProperty("updated_at")]
		public DateTimeOffset UpdatedAt { get; }
		
		[JsonProperty("review_at")]
		public DateTimeOffset ReviewAt { get; }
		
		[JsonProperty("closed_at")]
		public DateTimeOffset ClosedAt { get; }
		
		[JsonProperty("created_by")]
		public string CreatedBy { get; }
		
		[JsonProperty("description")]
		public string Description { get; }
		
		[JsonProperty("crm_enquiry")]
		public string CrmEnquiry { get; }
		
		[JsonProperty("trust_ukprn")]
		public string TrustUkPrn { get; }
		
		[JsonProperty("reason_at_review")]
		public string ReasonAtReview { get; }

		[JsonProperty("de_escalation")]
		public DateTimeOffset DeEscalation { get; }
		
		[JsonProperty("issue")]
		public string Issue { get; }

		[JsonProperty("current_status")]
		public string CurrentStatus { get; }

		[JsonProperty("case_aim")]
		public string CaseAim { get; }
		
		[JsonProperty("de_escalation_point")]
		public string DeEscalationPoint { get; }
		
		[JsonProperty("next_steps")]
		public string NextSteps { get; }
		
		/// <summary>
		/// Deteriorating, unchanged, improved
		/// </summary>
		[JsonProperty("direction_of_travel")]
		public string DirectionOfTravel { get; }
		
		[JsonProperty("urn")]
		public long Urn { get; } // TODO Remove property when TRAMS API is live
		
		[JsonProperty("status")]
		public long Status { get; }

		[JsonConstructor]
		public CreateCaseDto(DateTimeOffset createdAt, DateTimeOffset updatedAt, DateTimeOffset reviewAt, 
			DateTimeOffset closedAt, string createdBy, string description, string crmEnquiry, string trustUkPrn, 
			string reasonAtReview, DateTimeOffset deEscalation, string issue, string currentStatus, 
			string nextSteps, string caseAim, string deEscalationPoint, string directionOfTravel, long urn, long status) => 
			(CreatedAt, UpdatedAt, ReviewAt, ClosedAt, CreatedBy, Description, CrmEnquiry, TrustUkPrn,
				ReasonAtReview, DeEscalation, Issue, CurrentStatus, NextSteps, CaseAim, DeEscalationPoint, DirectionOfTravel, 
				Urn, Status) = 
			(createdAt, updatedAt, reviewAt, closedAt, createdBy, description, crmEnquiry, trustUkPrn,
				reasonAtReview, deEscalation, issue, currentStatus, nextSteps, caseAim, deEscalationPoint, directionOfTravel, 
				urn, status);
	}
}