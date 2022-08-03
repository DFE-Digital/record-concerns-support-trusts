using Newtonsoft.Json;
using System;

namespace Service.TRAMS.FinancialPlan
{
	public sealed class FinancialPlanStatusDto
	{
		[JsonProperty("id")]
		public long Id { get; }
		
		[JsonProperty("name")]
		public string Name { get; }
		
		[JsonProperty("createdAt")]
		public DateTimeOffset CreatedAt { get; }
		
		[JsonProperty("updatedAt")]
		public DateTimeOffset UpdatedAt { get; }
		
		[JsonProperty("description")]
		public string Description { get; }
		
		[JsonProperty("isClosedStatus")]
		public bool IsClosedStatus { get; }
		
		public FinancialPlanStatusDto(string name, string description, long id, bool isClosedStatus, DateTime createdAt, DateTime updatedAt) 
			=> (Name, Description, Id, IsClosedStatus, CreatedAt, UpdatedAt) 
				= (name, description, id, isClosedStatus, createdAt, updatedAt);
	}
}