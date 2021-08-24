using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Service.TRAMS.Models
{
	public sealed class TrustDto
	{		
		[JsonPropertyName("ukprn")]
		public string UkPrn { get; }
		
		[JsonPropertyName("urn")]
		public string Urn { get; }
		
		[JsonPropertyName("groupName")]
		public string GroupName { get; }
		
		[JsonPropertyName("companiesHouseNumber")]
		public string CompaniesHouseNumber { get; }
		
		[JsonPropertyName("establishments")]
		public List<EstablishmentDto> Establishments { get; }

		[JsonConstructor]
		public TrustDto(string ukprn, string urn, string groupName, string companiesHouseNumber, List<EstablishmentDto> establishments) => 
			(UkPrn, Urn, GroupName, CompaniesHouseNumber, Establishments) = (ukprn, urn, groupName, companiesHouseNumber, establishments);
	}
}