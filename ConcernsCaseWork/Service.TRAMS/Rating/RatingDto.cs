using Newtonsoft.Json;
using System;

namespace Service.TRAMS.Rating
{
	public sealed class RatingDto
	{
		/// <summary>
		/// n/a, Red-Plus, Red, Red-Amber, Amber-Green
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; }
		
		[JsonProperty("created_at")]
		public DateTimeOffset CreatedAt { get; }
		
		[JsonProperty("updated_at")]
		public DateTimeOffset UpdatedAt { get; }
		
		[JsonProperty("urn")]
		public long Urn { get; }
		
		public RatingDto(string name, DateTimeOffset createdAt, DateTimeOffset updatedAt, long urn) => 
			(Name, CreatedAt, UpdatedAt, Urn) = (name, createdAt, updatedAt, urn);
	}
}