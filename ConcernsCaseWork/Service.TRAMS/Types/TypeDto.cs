using Newtonsoft.Json;
using System;

namespace Service.TRAMS.Types
{
	public sealed class TypeDto
	{
		/// <summary>
		/// Compliance, Irregularity, Financial
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; }
		
		/// <summary>
		/// Financial reporting, Allegations and self reported concerns, Clawback
		/// </summary>
		[JsonProperty("description")]
		public string Description { get; }
		
		[JsonProperty("created_at")]
		public DateTimeOffset CreatedAt { get; }
		
		[JsonProperty("updated_at")]
		public DateTimeOffset UpdatedAt { get; }
		
		[JsonProperty("urn")]
		public long Urn { get; }
		
		[JsonConstructor]
		public TypeDto(string name, string description, DateTimeOffset createdAt, DateTimeOffset updatedAt, long urn) => 
			(Name, Description, CreatedAt, UpdatedAt, Urn) = (name, description, createdAt, updatedAt, urn);
	}
}