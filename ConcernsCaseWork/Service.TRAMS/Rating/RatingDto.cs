using System;
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
		public DateTimeOffset CreatedAt { get; }
		
		[JsonPropertyName("updated_at")]
		public DateTimeOffset UpdatedAt { get; }
		
		[JsonPropertyName("urn")]
		public long Urn { get; }
		
		public RatingDto(string name, DateTimeOffset createdAt, DateTimeOffset updatedAt, long urn) => 
			(Name, CreatedAt, UpdatedAt, Urn) = (name, createdAt, updatedAt, urn);
	}
}