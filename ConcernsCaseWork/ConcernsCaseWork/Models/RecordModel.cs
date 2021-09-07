using System;
using System.Numerics;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class RecordModel
	{
		public int Id { get; }

		public DateTimeOffset CreatedAt { get; }

		public DateTimeOffset UpdateAt { get; }

		public DateTimeOffset ReviewAt { get; }

		public DateTimeOffset ClosedAt { get; }

		public string Name { get; }

		public string Description { get; }
		
		public string Reason { get; }
	
		public int CaseId { get; }
		
		public int TypeId { get; }
		
		public int RatingId { get; }
		
		public bool Primary { get; }
		
		public BigInteger Urn { get; }
		
		public int Status { get; }
		
		public RecordModel(int id, DateTimeOffset createdAt, DateTimeOffset updatedAt, DateTimeOffset reviewAt, 
			DateTimeOffset closedAt, string name, string description, string reason, int caseId, int typeId, 
			int ratingId, bool primary, BigInteger urn, int status) => 
			(Id, CreatedAt, UpdateAt, ReviewAt, ClosedAt, Name, Description, Reason, CaseId, TypeId, RatingId, Primary, Urn, Status) = 
			(id, createdAt, updatedAt, reviewAt, closedAt, name, description, reason, caseId, typeId, ratingId, primary, urn, status);
	}
}