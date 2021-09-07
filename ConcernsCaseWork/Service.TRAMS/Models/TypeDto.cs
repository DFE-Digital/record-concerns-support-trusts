using System;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Service.TRAMS.Models
{
	public sealed class TypeDto
	{
		[JsonPropertyName("id")]
		public int Id { get; }
		
		[JsonPropertyName("name")]
		public string Name { get; }
		
		[JsonPropertyName("description")]
		public string Description { get; }
		
		[JsonPropertyName("created_at")]
		public DateTimeOffset CreatedAt { get; }
		
		[JsonPropertyName("updated_at")]
		public DateTimeOffset UpdatedAt { get; }
		
		[JsonPropertyName("urn")]
		public BigInteger Urn { get; }
		
		public TypeDto(int id, string name, string description, DateTimeOffset createdAt, 
			DateTimeOffset updatedAt, BigInteger urn) => 
			(Id, Name, Description, CreatedAt, UpdatedAt, Urn) = (id, name, description, createdAt, updatedAt, urn);
	}
}