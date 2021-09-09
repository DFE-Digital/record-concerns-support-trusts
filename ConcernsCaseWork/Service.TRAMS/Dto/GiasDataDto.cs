using System.Text.Json.Serialization;

namespace Service.TRAMS.Cases
{
	public sealed class GiasDataDto
	{
		[JsonPropertyName("ukprn")]
		public string UkPrn { get; }
			
		[JsonPropertyName("groupId")]
		public string GroupId { get; }
			
		[JsonPropertyName("groupName")]
		public string GroupName { get; }
		
		[JsonPropertyName("groupTypeCode")]
		public string GroupTypeCode { get; }
			
		[JsonPropertyName("companiesHouseNumber")]
		public string CompaniesHouseNumber { get; }
			
		[JsonPropertyName("groupContactAddress")]
		public GroupContactAddressDto GroupContactAddress { get; }
			
		[JsonConstructor]
		public GiasDataDto(string ukprn, string groupId, string groupName, string groupTypeCode, string companiesHouseNumber, GroupContactAddressDto groupContactAddress) => 
			(UkPrn, GroupId, GroupName, GroupTypeCode, CompaniesHouseNumber, GroupContactAddress) = (ukprn, groupId, groupName, groupTypeCode, companiesHouseNumber, groupContactAddress);
	}
}