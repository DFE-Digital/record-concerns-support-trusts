using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Types
{
	public sealed class TypeDto
	{
		/// <summary>
		/// Compliance, Irregularity, Financial
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; }
		
		/// <summary>
		/// Financial reporting, Allegations and self reported concerns, Clawback
		/// </summary>
		[JsonProperty("description")]
		public string Description { get; }
		
		[JsonProperty("created_at")]
		public DateTimeOffset CreatedAt { get; }
		
		[JsonProperty("updated_at")]
		public DateTimeOffset UpdatedAt { get; }
		
		[JsonProperty("id")]
		public long Id { get; }
		
		[JsonConstructor]
		public TypeDto(string name, string description, DateTimeOffset createdAt, DateTimeOffset updatedAt, long id) => 
			(Name, Description, CreatedAt, UpdatedAt, Id) = (name, description, createdAt, updatedAt, id);
	}
}