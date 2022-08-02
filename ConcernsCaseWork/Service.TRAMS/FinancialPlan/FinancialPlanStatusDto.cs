using Newtonsoft.Json;
using System;

namespace Service.TRAMS.FinancialPlan
{
	public sealed class FinancialPlanStatusDto
	{
		[JsonProperty("name")]
		public string Name { get; }
		
		[JsonProperty("id")]
		public long Id { get; }
		
		[JsonProperty("description")]
		public string Description { get; }
		
		[JsonProperty("isClosedStatus")]
		public bool IsClosedStatus { get; }
		
		public FinancialPlanStatusDto(string name, string description, long id, bool isClosedStatus) => (Name, Description, Id, IsClosedStatus) = (name, description, id, isClosedStatus);
	}
}