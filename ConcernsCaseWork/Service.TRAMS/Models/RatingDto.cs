using System;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Service.TRAMS.Models
{
	public sealed class RatingDto
	{
		[JsonPropertyName("id")]
		public int Id { get; }
		
		/// <summary>
		/// n/a, Red-Plus, Red, Red-Amber, Amber-Green
		/// </summary>
		[JsonPropertyName("name")]
		public string Name { get; }
		
		[JsonPropertyName("created_at")]
		public DateTimeOffset CreatedAt { get; }
		
		[JsonPropertyName("updated_at")]
		public DateTimeOffset UpdatedAt { get; }
		
		[JsonPropertyName("urn")]
		public BigInteger Urn { get; }
		
		public RatingDto(int id, string name, DateTimeOffset createdAt, DateTimeOffset updatedAt, BigInteger urn) => 
			(Id, Name, CreatedAt, UpdatedAt, Urn) = (id, name, createdAt, updatedAt, urn);
	}
}