using System;
using System.Numerics;

namespace ConcernsCaseWork.Models
{
	public sealed class StatusModel
	{
		/// <summary>
		/// Live, Monitoring, Close
		/// </summary>
		public string Name { get; }
		
		public DateTimeOffset CreatedAt { get; }
		
		public DateTimeOffset UpdatedAt { get; }
		
		public BigInteger Urn { get; }
		
		public StatusModel(string name, DateTimeOffset createdAt, DateTimeOffset updatedAt, BigInteger urn) => 
			(Name, CreatedAt, UpdatedAt, Urn) = (name, createdAt, updatedAt, urn);
	}
}