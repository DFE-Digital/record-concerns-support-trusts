using System;
using System.Numerics;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class CaseModel
	{
		public int Id { get; }
		
		public DateTimeOffset CreatedAt { get; }

		public DateTimeOffset UpdateAt { get; }
		
		public DateTimeOffset ReviewAt { get; }

		public DateTimeOffset ClosedAt { get; }
		
		public string CreatedBy { get; }
		
		public string Description { get; }
		
		public string CrmEnquiry { get; }
		
		public string TrustUkPrn { get; }
		
		public string ReasonAtReview { get; }
		
		public DateTimeOffset DeEscalation { get; }
		
		public string Issue { get; }
		
		public string CurrentStatus { get; }

		public string NextSteps { get; }
		
		public string ResolutionStrategy { get; }
		
		public string DirectionOfTravel { get; }
		
		public BigInteger Urn { get; }
		
		public int Status { get; }

		public CaseModel(int id, DateTimeOffset createdAt, DateTimeOffset updatedAt, DateTimeOffset reviewAt, 
			DateTimeOffset closedAt, string createdBy, string description, string crmEnquiry, string trustUkPrn, 
			string reasonAtReview, DateTimeOffset deEscalation, string issue, string currentStatus, string nextSteps,
			string resolutionStrategy, string directionOfTravel, BigInteger urn, int status) => 
			(Id, CreatedAt, UpdateAt, ReviewAt, ClosedAt, CreatedBy, Description, CrmEnquiry, TrustUkPrn, 
				ReasonAtReview, DeEscalation, Issue, CurrentStatus, NextSteps, ResolutionStrategy, DirectionOfTravel, Urn, Status) = 
			(id, createdAt, updatedAt, reviewAt, closedAt, createdBy, description, crmEnquiry, trustUkPrn, reasonAtReview, deEscalation,
				issue, currentStatus, nextSteps, resolutionStrategy, directionOfTravel, urn, status);
	}
}