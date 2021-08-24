using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Service.TRAMS.Models
{
	public sealed class TrustDto
	{		
		public string UkPrn { get; }
		public string Urn { get; }
		public string GroupName { get; }
		public string CompaniesHouseNumber { get; }
		public List<EstablishmentDto> Establishments { get; }

		[JsonConstructor]
		public TrustDto(string ukprn, string urn, string groupName, string companiesHouseNumber, List<EstablishmentDto> establishments) => 
			(UkPrn, Urn, GroupName, CompaniesHouseNumber, Establishments) = (ukprn, urn, groupName, companiesHouseNumber, establishments);
	}
}