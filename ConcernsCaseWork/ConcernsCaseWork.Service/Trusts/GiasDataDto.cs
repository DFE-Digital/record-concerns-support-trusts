using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Trusts
{
	public class GiasDataDto
	{
		[JsonProperty("ukprn")]
		public virtual string UkPrn { get; set; }
			
		[JsonProperty("groupId")]
		public virtual string GroupId { get; set; }
			
		[JsonProperty("groupName")]
		public virtual string GroupName { get; set; }
		
		[JsonProperty("groupType")]
		public virtual string GroupType { get; set; }
		
		[JsonProperty("groupTypeCode")]
		public virtual string GroupTypeCode { get; set; }
			
		[JsonProperty("companiesHouseNumber")]
		public virtual string CompaniesHouseNumber { get; set; }
			
		[JsonProperty("groupContactAddress")]
		public virtual GroupContactAddressDto GroupContactAddress { get; set; }
			
		[JsonConstructor]
		public GiasDataDto(string ukprn, string groupId, string groupName, string groupTypeCode, string companiesHouseNumber, GroupContactAddressDto groupContactAddress, string groupType) => 
			(UkPrn, GroupId, GroupName, GroupTypeCode, CompaniesHouseNumber, GroupContactAddress, GroupType) = (ukprn, groupId, groupName, groupTypeCode, companiesHouseNumber, groupContactAddress, groupType);
		
		public GiasDataDto() { }
	}
}