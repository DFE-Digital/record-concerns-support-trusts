using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Trusts
{
	public sealed class TrustSearchDto
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

		[JsonConstructor]
		public TrustSearchDto(string ukprn, string urn, string groupName, 
			string companiesHouseNumber, string trustType, GroupContactAddressDto groupContactAddress) => 
			(UkPrn, Urn, GroupName, CompaniesHouseNumber, TrustType, GroupContactAddress) = 
			(ukprn, urn, groupName, companiesHouseNumber, trustType, groupContactAddress);
	}
}