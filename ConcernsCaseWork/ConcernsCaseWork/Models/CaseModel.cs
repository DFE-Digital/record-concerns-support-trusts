using System;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class CaseModel
	{
		public DateTimeOffset CreatedAt { get; set; }

		public DateTimeOffset UpdatedAt { get; set; }

		public DateTimeOffset ReviewAt { get; set; }

		public DateTimeOffset ClosedAt { get; set; }
		
		/// <summary>
		/// Case owner from azure AD some unique identifier
		/// </summary>
		public string CreatedBy { get; set; }

		public string Description { get; set; }

		public string CrmEnquiry { get; set; }

		public string TrustUkPrn { get; set; }
		
		public string ReasonAtReview { get; set; }

		public DateTimeOffset DeEscalation { get; set; }

		public string Issue { get; set; }

		public string CurrentStatus { get; set; }

		public string NextSteps { get; set; }
	
		public string ResolutionStrategy { get; set; }
		
		/// <summary>
		/// Deteriorating, unchanged, improved
		/// </summary>
		public string DirectionOfTravel { get; set; }

		public long Urn { get; set; }

		public long Status { get; set; }
		
		public string StatusName { get; set; }

		// public CaseModel(DateTimeOffset createdAt, DateTimeOffset updatedAt, DateTimeOffset reviewAt, DateTimeOffset closedAt, 
		// 	string createdBy, string description, string crmEnquiry, string trustUkPrn, string reasonAtReview, 
		// 	DateTimeOffset deEscalation, string issue, string currentStatus, string nextSteps, string resolutionStrategy, 
		// 	string directionOfTravel, long urn, string status) => 
		// 	(CreatedAt, UpdatedAt, ReviewAt, ClosedAt, CreatedBy, Description, CrmEnquiry, TrustUkPrn,
		// 		ReasonAtReview, DeEscalation, Issue, CurrentStatus, NextSteps, ResolutionStrategy, DirectionOfTravel, 
		// 		Urn, Status) = 
		// 	(createdAt, updatedAt, reviewAt, closedAt, createdBy, description, crmEnquiry, trustUkPrn,
		// 		reasonAtReview, deEscalation, issue, currentStatus, nextSteps, resolutionStrategy, directionOfTravel, 
		// 		urn, status);
	}
}