using Newtonsoft.Json;

namespace ConcernsCasework.Service.Status
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
		
		[JsonProperty("urn")]
		public long Urn { get; }
		
		[JsonConstructor]
		public StatusDto(string name, DateTimeOffset createdAt, DateTimeOffset updatedAt, long urn) => 
			(Name, CreatedAt, UpdatedAt, Urn) = (name, createdAt, updatedAt, urn);
	}
}