using System;

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
		
		public long Urn { get; }
		
		public StatusModel(string name, DateTimeOffset createdAt, DateTimeOffset updatedAt, long urn) => 
			(Name, CreatedAt, UpdatedAt, Urn) = (name, createdAt, updatedAt, urn);
	}
}