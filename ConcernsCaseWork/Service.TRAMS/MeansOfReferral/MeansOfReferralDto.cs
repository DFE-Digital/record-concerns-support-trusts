using Newtonsoft.Json;

namespace Service.TRAMS.MeansOfReferral
{
	public sealed class MeansOfReferralDto
	{
		[JsonProperty("name")]
		public string Name { get; set; }
		
		[JsonProperty("description")]
		public string Description { get; set; }
		
		[JsonProperty("urn")]
		public long Urn { get; set; }
		
		public MeansOfReferralDto()
		{
		}
		
		public MeansOfReferralDto(string name, string description, long urn)
		{
			Name = name;
			Description = description;
			Urn = urn;
		}
	}
}