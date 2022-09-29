using Newtonsoft.Json;
using System;

namespace Service.TRAMS.Status
{
	public sealed class StatusDto
	{
		/// <summary>
		/// Live, Monitoring, Close
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; }
		
		[JsonProperty("createdAt")]
		public DateTimeOffset CreatedAt { get; }
		
		[JsonProperty("updatedAt")]
		public DateTimeOffset UpdatedAt { get; }
		
		[JsonProperty("id")]
		public long Id { get; }
		
		[JsonConstructor]
		public StatusDto(string name, DateTimeOffset createdAt, DateTimeOffset updatedAt, long id) => 
			(Name, CreatedAt, UpdatedAt, Id) = (name, createdAt, updatedAt, id);
	}
}