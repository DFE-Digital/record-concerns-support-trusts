using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Trusts
{
	public sealed class GiasDataDto
	{
		[JsonProperty("ukprn")]
		public string UkPrn { get; }
			
		[JsonProperty("groupId")]
		public string GroupId { get; }
			
		[JsonProperty("groupName")]
		public string GroupName { get; }
		
		[JsonProperty("groupType")]
		public string GroupType { get; }
		
		[JsonProperty("groupTypeCode")]
		public string GroupTypeCode { get; }
			
		[JsonProperty("companiesHouseNumber")]
		public string CompaniesHouseNumber { get; }
			
		[JsonProperty("groupContactAddress")]
		public GroupContactAddressDto GroupContactAddress { get; }
			
		[JsonConstructor]
		public GiasDataDto(string ukprn, string groupId, string groupName, string groupTypeCode, string companiesHouseNumber, GroupContactAddressDto groupContactAddress, string groupType) => 
			(UkPrn, GroupId, GroupName, GroupTypeCode, CompaniesHouseNumber, GroupContactAddress, GroupType) = (ukprn, groupId, groupName, groupTypeCode, companiesHouseNumber, groupContactAddress, groupType);
	}
}