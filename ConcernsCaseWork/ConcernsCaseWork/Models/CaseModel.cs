using System;
using System.Numerics;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class CaseModel
	{
		public DateTimeOffset CreatedAt { get; }

		public DateTimeOffset UpdateAt { get; }

		public DateTimeOffset ReviewAt { get; }

		public DateTimeOffset ClosedAt { get; }
		
		/// <summary>
		/// Case owner from azure AD some unique identifier
		/// </summary>
		public string CreatedBy { get; }

		public string Description { get; }

		public string CrmEnquiry { get; }

		public string TrustUkPrn { get; }

		public string TrustName { get; }

		public string ReasonAtReview { get; }

		public DateTimeOffset DeEscalation { get; }

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

		public CaseModel(DateTimeOffset createdAt, DateTimeOffset updatedAt, DateTimeOffset reviewAt, 
			DateTimeOffset closedAt, string createdBy, string description, string crmEnquiry, string trustUkPrn, 
			string trustName, string reasonAtReview, DateTimeOffset deEscalation, string issue, string currentStatus, 
			string nextSteps, string resolutionStrategy, string directionOfTravel, BigInteger urn, string status) => 
			(CreatedAt, UpdateAt, ReviewAt, ClosedAt, CreatedBy, Description, CrmEnquiry, TrustUkPrn, TrustName,
				ReasonAtReview, DeEscalation, Issue, CurrentStatus, NextSteps, ResolutionStrategy, DirectionOfTravel, 
				Urn, Status) = 
			(createdAt, updatedAt, reviewAt, closedAt, createdBy, description, crmEnquiry, trustUkPrn, trustName,
				reasonAtReview, deEscalation, issue, currentStatus, nextSteps, resolutionStrategy, directionOfTravel, 
				urn, status);
	}
}