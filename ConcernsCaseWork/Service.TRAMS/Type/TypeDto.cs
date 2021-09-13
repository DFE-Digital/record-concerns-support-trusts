using Newtonsoft.Json;
using System;

namespace Service.TRAMS.Type
{
	public sealed class TypeDto
	{
		/// <summary>
		/// Record, SRMA, Safeguarding, Concern
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; }
		
		/// <summary>
		/// Record (Log information when it is not a Concern)
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