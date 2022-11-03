using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Ratings
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
		
		[JsonProperty("id")]
		public long Id { get; }
		
		public RatingDto(string name, DateTimeOffset createdAt, DateTimeOffset updatedAt, long id) => 
			(Name, CreatedAt, UpdatedAt, Id) = (name, createdAt, updatedAt, id);
	}
}