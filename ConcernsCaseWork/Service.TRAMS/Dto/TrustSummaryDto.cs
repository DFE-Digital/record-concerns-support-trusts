using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Service.TRAMS.Cases
{
	public sealed class TrustSummaryDto
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
		public List<EstablishmentSummaryDto> Establishments { get; }

		[JsonConstructor]
		public TrustSummaryDto(string ukprn, string urn, string groupName, string companiesHouseNumber, List<EstablishmentSummaryDto> establishments) => 
			(UkPrn, Urn, GroupName, CompaniesHouseNumber, Establishments) = (ukprn, urn, groupName, companiesHouseNumber, establishments);
	}
}