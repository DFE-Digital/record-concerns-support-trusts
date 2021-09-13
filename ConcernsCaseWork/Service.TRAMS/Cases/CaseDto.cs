using System;
using System.Text.Json.Serialization;

namespace Service.TRAMS.Cases
{
	public sealed class CaseDto
	{
		[JsonPropertyName("created_at")]
		public DateTimeOffset CreatedAt { get; }

		[JsonPropertyName("updated_at")]
		public DateTimeOffset UpdateAt { get; }
		
		[JsonPropertyName("review_at")]
		public DateTimeOffset ReviewAt { get; }
		
		[JsonPropertyName("closed_at")]
		public DateTimeOffset ClosedAt { get; }
		
		[JsonPropertyName("created_by")]
		public string CreatedBy { get; }
		
		[JsonPropertyName("description")]
		public string Description { get; }
		
		[JsonPropertyName("crm_enquiry")]
		public string CrmEnquiry { get; }
		
		[JsonPropertyName("trust_ukprn")]
		public string TrustUkPrn { get; }
		
		[JsonPropertyName("reason_at_review")]
		public string ReasonAtReview { get; }

		[JsonPropertyName("de_escalation")]
		public DateTimeOffset DeEscalation { get; }
		
		[JsonPropertyName("issue")]
		public string Issue { get; }

		[JsonPropertyName("current_status")]
		public string CurrentStatus { get; }

		[JsonPropertyName("next_steps")]
		public string NextSteps { get; }
		
		[JsonPropertyName("resolution_strategy")]
		public string ResolutionStrategy { get; }
		
		/// <summary>
		/// Deteriorating, unchanged, improved
		/// </summary>
		[JsonPropertyName("direction_of_travel")]
		public string DirectionOfTravel { get; }
		
		[JsonPropertyName("urn")]
		public long Urn { get; set; } // TODO Remove setter when TRAMS API is live
		
		[JsonPropertyName("status")]
		public long Status { get; }

		[JsonConstructor]
		public CaseDto(DateTimeOffset createdAt, DateTimeOffset updatedAt, DateTimeOffset reviewAt, 
			DateTimeOffset closedAt, string createdBy, string description, string crmEnquiry, string trustUkPrn, 
			string reasonAtReview, DateTimeOffset deEscalation, string issue, string currentStatus, 
			string nextSteps, string resolutionStrategy, string directionOfTravel, long urn, long status) => 
			(CreatedAt, UpdateAt, ReviewAt, ClosedAt, CreatedBy, Description, CrmEnquiry, TrustUkPrn,
				ReasonAtReview, DeEscalation, Issue, CurrentStatus, NextSteps, ResolutionStrategy, DirectionOfTravel, 
				Urn, Status) = 
			(createdAt, updatedAt, reviewAt, closedAt, createdBy, description, crmEnquiry, trustUkPrn,
				reasonAtReview, deEscalation, issue, currentStatus, nextSteps, resolutionStrategy, directionOfTravel, 
				urn, status);
	}
}