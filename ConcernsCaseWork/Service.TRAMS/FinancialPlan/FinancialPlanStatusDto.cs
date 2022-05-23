using Newtonsoft.Json;
using System;

namespace Service.TRAMS.FinancialPlan
{
	public sealed class FinancialPlanStatusDto
	{
		[JsonProperty("name")]
		public string Name { get; }
		
		[JsonProperty("createdAt")]
		public DateTimeOffset CreatedAt { get; }
		
		[JsonProperty("updatedAt")]
		public DateTimeOffset UpdatedAt { get; }
		
		[JsonProperty("id")]
		public long Id { get; }
		
		public FinancialPlanStatusDto(string name, DateTimeOffset createdAt, DateTimeOffset updatedAt, long id) => 
			(Name, CreatedAt, UpdatedAt, Id) = (name, createdAt, updatedAt, id);
	}
}