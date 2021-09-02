using System.Text.Json.Serialization;

namespace Service.TRAMS.Models
{
	public sealed class GiasDataDto
	{
		[JsonPropertyName("ukprn")]
		public string UkPrn { get; }
			
		[JsonPropertyName("groupId")]
		public string GroupId { get; }
			
		[JsonPropertyName("groupName")]
		public string GroupName { get; }
			
		[JsonPropertyName("companiesHouseNumber")]
		public string CompaniesHouseNumber { get; }
			
		[JsonPropertyName("groupContactAddress")]
		public GroupContactAddressDto GroupContactAddress { get; }
			
		public GiasDataDto(string ukprn, string groupId, string groupName, string companiesHouseNumber, GroupContactAddressDto groupContactAddress) => 
			(UkPrn, GroupId, GroupName, CompaniesHouseNumber, GroupContactAddress) = (ukprn, groupId, groupName, companiesHouseNumber, groupContactAddress);
	}
}