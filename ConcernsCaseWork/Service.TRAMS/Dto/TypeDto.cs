using System;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Service.TRAMS.RecordAcademy
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
		public DateTimeOffset CreatedAt { get; }
		
		[JsonPropertyName("updated_at")]
		public DateTimeOffset UpdatedAt { get; }
		
		[JsonPropertyName("urn")]
		public BigInteger Urn { get; }
		
		[JsonConstructor]
		public TypeDto(string name, string description, DateTimeOffset createdAt, 
			DateTimeOffset updatedAt, BigInteger urn) => 
			(Name, Description, CreatedAt, UpdatedAt, Urn) = (name, description, createdAt, updatedAt, urn);
	}
}