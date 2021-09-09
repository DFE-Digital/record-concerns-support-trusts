using System;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Service.TRAMS.Rating
{
	public sealed class RatingDto
	{
		/// <summary>
		/// n/a, Red-Plus, Red, Red-Amber, Amber-Green
		/// </summary>
		[JsonPropertyName("name")]
		public string Name { get; }
		
		[JsonPropertyName("created_at")]
		public DateTime CreatedAt { get; }
		
		[JsonPropertyName("updated_at")]
		public DateTime UpdatedAt { get; }
		
		[JsonPropertyName("urn")]
		public BigInteger Urn { get; }
		
		public RatingDto(string name, DateTime createdAt, DateTime updatedAt, BigInteger urn) => 
			(Name, CreatedAt, UpdatedAt, Urn) = (name, createdAt, updatedAt, urn);
	}
}