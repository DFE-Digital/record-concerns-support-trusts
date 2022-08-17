using Newtonsoft.Json;

namespace ConcernsCasework.Service.Ratings
{
	public sealed class RatingDto
	{
		/// <summary>
		/// n/a, Red-Plus, Red, Red-Amber, Amber-Green
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; }
		
		[JsonProperty("createdAt")]
		public DateTimeOffset CreatedAt { get; }
		
		[JsonProperty("updatedAt")]
		public DateTimeOffset UpdatedAt { get; }
		
		[JsonProperty("urn")]
		public long Urn { get; }
		
		public RatingDto(string name, DateTimeOffset createdAt, DateTimeOffset updatedAt, long urn) => 
			(Name, CreatedAt, UpdatedAt, Urn) = (name, createdAt, updatedAt, urn);
	}
}