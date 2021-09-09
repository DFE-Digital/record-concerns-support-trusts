using System;
using System.Numerics;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class CaseModel
	{
		public DateTime CreatedAt { get; }

		public DateTime UpdateAt { get; }

		public DateTime ReviewAt { get; }

		public DateTime ClosedAt { get; }
		
		/// <summary>
		/// Case owner from azure AD some unique identifier
		/// </summary>
		public string CreatedBy { get; }

		public string Description { get; }

		public string CrmEnquiry { get; }

		public string TrustUkPrn { get; }
		
		public string ReasonAtReview { get; }

		public DateTime DeEscalation { get; }

		public string Issue { get; }

		public string CurrentStatus { get; }

		public string NextSteps { get; }
	
		public string ResolutionStrategy { get; }
		
		/// <summary>
		/// Deteriorating, unchanged, improved
		/// </summary>
		public string DirectionOfTravel { get; }

		public BigInteger Urn { get; }

		public string Status { get; }

		public CaseModel(DateTime createdAt, DateTime updatedAt, DateTime reviewAt, DateTime closedAt, 
			string createdBy, string description, string crmEnquiry, string trustUkPrn, string reasonAtReview, 
			DateTime deEscalation, string issue, string currentStatus, string nextSteps, string resolutionStrategy, 
			string directionOfTravel, BigInteger urn, string status) => 
			(CreatedAt, UpdateAt, ReviewAt, ClosedAt, CreatedBy, Description, CrmEnquiry, TrustUkPrn,
				ReasonAtReview, DeEscalation, Issue, CurrentStatus, NextSteps, ResolutionStrategy, DirectionOfTravel, 
				Urn, Status) = 
			(createdAt, updatedAt, reviewAt, closedAt, createdBy, description, crmEnquiry, trustUkPrn,
				reasonAtReview, deEscalation, issue, currentStatus, nextSteps, resolutionStrategy, directionOfTravel, 
				urn, status);
	}
}