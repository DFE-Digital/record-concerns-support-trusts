using System;
using System.Text.Json.Serialization;

namespace Service.TRAMS.Status
{
	public sealed class StatusDto
	{
		/// <summary>
		/// Live, Monitoring, Close
		/// </summary>
		[JsonPropertyName("name")]
		public string Name { get; }
		
		[JsonPropertyName("created_at")]
		public DateTimeOffset CreatedAt { get; }
		
		[JsonPropertyName("updated_at")]
		public DateTimeOffset UpdatedAt { get; }
		
		[JsonPropertyName("urn")]
		public long Urn { get; }
		
		[JsonConstructor]
		public StatusDto(string name, DateTimeOffset createdAt, DateTimeOffset updatedAt, long urn) => 
			(Name, CreatedAt, UpdatedAt, Urn) = (name, createdAt, updatedAt, urn);
	}
}