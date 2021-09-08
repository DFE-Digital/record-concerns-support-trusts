using System;
using System.Numerics;

namespace ConcernsCaseWork.Models
{
	public sealed class RatingModel
	{
		/// <summary>
		/// n/a, Red-Plus, Red, Red-Amber, Amber-Green
		/// </summary>
		public string Name { get; }
		
		public DateTimeOffset CreatedAt { get; }
		
		public DateTimeOffset UpdatedAt { get; }
		
		public BigInteger Urn { get; }
		
		public RatingModel(string name, DateTimeOffset createdAt, DateTimeOffset updatedAt, BigInteger urn) => 
			(Name, CreatedAt, UpdatedAt, Urn) = (name, createdAt, updatedAt, urn);
	}
}