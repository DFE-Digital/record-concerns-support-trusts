using System.Text.Json.Serialization;

namespace Service.TRAMS.Models
{
	public sealed class EstablishmentDto
	{
		[JsonPropertyName("urn")]
		public string Urn { get; }
		
		[JsonPropertyName("name")]
		public string Name { get; }
		
		[JsonPropertyName("ukprn")]
		public string UkPrn { get; }
		
		[JsonConstructor]
		public EstablishmentDto(string urn, string name, string ukprn) => 
			(Urn, Name, UkPrn) = (urn, name, ukprn);
	}
}