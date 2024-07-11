using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Trusts
{
	public class TrustSearchV4Dto
	{
		[JsonProperty("ukprn")]
		public virtual string UkPrn { get; set; }

		[JsonProperty("urn")]
		public virtual string Urn { get; set; }

		[JsonProperty("name")]
		public virtual string GroupName { get; set; }

		[JsonProperty("companiesHouseNumber")]
		public virtual string CompaniesHouseNumber { get; set; }

		[JsonProperty("trustType")]
		public virtual string TrustType { get; set; }

		[JsonProperty("address")]
		public virtual GroupContactAddressDto GroupContactAddress { get; set; }

		public TrustSearchV4Dto() { }
	}
}
