using Newtonsoft.Json;
using System.Collections.Generic;

namespace Service.TRAMS.Trusts
{
	public sealed class TrustSummaryDto
	{		
		[JsonProperty("ukprn")]
		public string UkPrn { get; }
		
		[JsonProperty("urn")]
		public string Urn { get; }
		
		[JsonProperty("groupName")]
		public string GroupName { get; }
		
		[JsonProperty("companiesHouseNumber")]
		public string CompaniesHouseNumber { get; }
		
		[JsonProperty("trustType")]
		public string TrustType { get; }
		
		[JsonProperty("trustAddress")]
		public GroupContactAddressDto GroupContactAddress { get; }
		
		[JsonProperty("establishments")]
		public List<EstablishmentSummaryDto> Establishments { get; }

		[JsonConstructor]
		public TrustSummaryDto(string ukprn, string urn, string groupName, 
			string companiesHouseNumber, string trustType, GroupContactAddressDto groupContactAddress,
			List<EstablishmentSummaryDto> establishments) => 
			(UkPrn, Urn, GroupName, CompaniesHouseNumber, TrustType, GroupContactAddress, Establishments) = 
			(ukprn, urn, groupName, companiesHouseNumber, trustType, groupContactAddress, establishments);
	}
}