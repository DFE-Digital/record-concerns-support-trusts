using Newtonsoft.Json;

namespace Service.TRAMS.MeansOfReferral
{
	public sealed class MeansOfReferralDto
	{
		[JsonProperty("name")]
		public string Name { get; set; }
		
		[JsonProperty("description")]
		public string Description { get; set; }
		
		[JsonProperty("id")]
		public long Id { get; set; }
		
		public MeansOfReferralDto(string name, string description, long id)
		{
			Name = name;
			Description = description;
			Id = id;
		}
	}
}