using Newtonsoft.Json;

namespace Service.TRAMS.Trusts
{
	public sealed class EstablishmentDto
	{
		[JsonProperty("urn")]
		public string Urn { get; }
		
		[JsonProperty("localAuthorityCode")]
		public string LocalAuthorityCode { get; }
		
		[JsonProperty("localAuthorityName")]
		public string LocalAuthorityName { get; }
		
		[JsonProperty("establishmentNumber")]
		public string EstablishmentNumber { get; }
		
		[JsonProperty("establishmentName")]
		public string EstablishmentName { get; }
		
		[JsonConstructor]
		public EstablishmentDto(string urn, string localAuthorityCode, string localAuthorityName,
			string establishmentNumber, string establishmentName) => 
			(Urn, LocalAuthorityCode, LocalAuthorityName, EstablishmentNumber, EstablishmentName) = 
			(urn, localAuthorityCode, localAuthorityName, establishmentNumber, establishmentName);
	}
}