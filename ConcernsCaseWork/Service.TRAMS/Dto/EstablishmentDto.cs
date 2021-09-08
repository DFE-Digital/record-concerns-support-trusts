using System.Text.Json.Serialization;

namespace Service.TRAMS.Dto
{
	public sealed class EstablishmentDto
	{
		[JsonPropertyName("urn")]
		public string Urn { get; }
		
		[JsonPropertyName("localAuthorityCode")]
		public string LocalAuthorityCode { get; }
		
		[JsonPropertyName("localAuthorityName")]
		public string LocalAuthorityName { get; }
		
		[JsonPropertyName("establishmentNumber")]
		public string EstablishmentNumber { get; }
		
		[JsonPropertyName("establishmentName")]
		public string EstablishmentName { get; }
		
		[JsonConstructor]
		public EstablishmentDto(string urn, string localAuthorityCode, string localAuthorityName,
			string establishmentNumber, string establishmentName) => 
			(Urn, LocalAuthorityCode, LocalAuthorityName, EstablishmentNumber, EstablishmentName) = 
			(urn, localAuthorityCode, localAuthorityName, establishmentNumber, establishmentName);
	}
}