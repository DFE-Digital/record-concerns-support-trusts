using System;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Service.TRAMS.Type
{
	public sealed class TypeDto
	{
		/// <summary>
		/// Record, SRMA, Safeguarding, Concern
		/// </summary>
		[JsonPropertyName("name")]
		public string Name { get; }
		
		/// <summary>
		/// Record (Log information when it is not a Concern)
		/// </summary>
		[JsonPropertyName("description")]
		public string Description { get; }
		
		[JsonPropertyName("created_at")]
		public DateTime CreatedAt { get; }
		
		[JsonPropertyName("updated_at")]
		public DateTime UpdatedAt { get; }
		
		[JsonPropertyName("urn")]
		public BigInteger Urn { get; }
		
		[JsonConstructor]
		public TypeDto(string name, string description, DateTime createdAt, 
			DateTime updatedAt, BigInteger urn) => 
			(Name, Description, CreatedAt, UpdatedAt, Urn) = (name, description, createdAt, updatedAt, urn);
	}
}