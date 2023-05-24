using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Trusts
{
	public class TrustSearchDto
	{		
		[JsonProperty("ukprn")]
		public virtual string UkPrn { get; set; }
		
		[JsonProperty("urn")]
		public virtual string Urn { get; set; }
		
		[JsonProperty("groupName")]
		public virtual string GroupName { get; set; }
		
		[JsonProperty("companiesHouseNumber")]
		public virtual string CompaniesHouseNumber { get; set; }
		
		[JsonProperty("trustType")]
		public virtual string TrustType { get; set; }
		
		[JsonProperty("trustAddress")]
		public virtual GroupContactAddressDto GroupContactAddress { get; set; }

		[JsonConstructor]
		public TrustSearchDto(string ukprn, string urn, string groupName, 
			string companiesHouseNumber, string trustType, GroupContactAddressDto groupContactAddress) => 
			(UkPrn, Urn, GroupName, CompaniesHouseNumber, TrustType, GroupContactAddress) = 
			(ukprn, urn, groupName, companiesHouseNumber, trustType, groupContactAddress);
		
		public TrustSearchDto() { }
	}
}