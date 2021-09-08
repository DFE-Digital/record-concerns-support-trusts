using System;
using System.Numerics;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class RecordModel
	{
		public DateTimeOffset CreatedAt { get; }

		public DateTimeOffset UpdateAt { get; }

		public DateTimeOffset ReviewAt { get; }

		public DateTimeOffset ClosedAt { get; }

		public string Name { get; }
	
		public string Description { get; }

		public string Reason { get; }
	
		public BigInteger CaseUrn { get; }

		public BigInteger TypeUrn { get; }

		public BigInteger RatingUrn { get; }
		
		public bool Primary { get; }
		
		public BigInteger Urn { get; }
		
		public int Status { get; }
		
		public RecordModel(DateTimeOffset createdAt, DateTimeOffset updatedAt, DateTimeOffset reviewAt, 
			DateTimeOffset closedAt, string name, string description, string reason, BigInteger caseUrn, BigInteger typeUrn, 
			BigInteger ratingUrn, bool primary, BigInteger urn, int status) => 
			(CreatedAt, UpdateAt, ReviewAt, ClosedAt, Name, Description, Reason, CaseUrn, TypeUrn, RatingUrn, Primary, Urn, Status) = 
			(createdAt, updatedAt, reviewAt, closedAt, name, description, reason, caseUrn, typeUrn, ratingUrn, primary, urn, status);
	}
}