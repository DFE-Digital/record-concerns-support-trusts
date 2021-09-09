using System;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Service.TRAMS.Status
{
	public sealed class UpdateCaseDto
	{
		[JsonPropertyName("updated_at")]
		public DateTime UpdateAt { get; }
		
		[JsonPropertyName("review_at")]
		public DateTime ReviewAt { get; }
		
		[JsonPropertyName("closed_at")]
		public DateTime ClosedAt { get; }
		
		/// <summary>
		/// Case owner from azure AD some unique identifier
		/// </summary>
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
		public DateTime DeEscalation { get; }
		
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
		public BigInteger Urn { get; }
		
		[JsonPropertyName("status")]
		public BigInteger Status { get; }

		[JsonConstructor]
		public UpdateCaseDto(DateTime updatedAt, DateTime reviewAt, DateTime closedAt, 
			string createdBy, string description, string crmEnquiry, string trustUkPrn, string reasonAtReview, 
			DateTime deEscalation, string issue, string currentStatus, string nextSteps, string resolutionStrategy, 
			string directionOfTravel, BigInteger urn, BigInteger status) => 
			(UpdateAt, ReviewAt, ClosedAt, CreatedBy, Description, CrmEnquiry, TrustUkPrn,
				ReasonAtReview, DeEscalation, Issue, CurrentStatus, NextSteps, ResolutionStrategy, DirectionOfTravel, 
				Urn, Status) = 
			(updatedAt, reviewAt, closedAt, createdBy, description, crmEnquiry, trustUkPrn,
				reasonAtReview, deEscalation, issue, currentStatus, nextSteps, resolutionStrategy, directionOfTravel, 
				urn, status);
	}
}