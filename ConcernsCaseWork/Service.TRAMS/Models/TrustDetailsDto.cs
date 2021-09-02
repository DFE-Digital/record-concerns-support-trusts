using System.Text.Json.Serialization;

namespace Service.TRAMS.Models
{
	public sealed class TrustDetailsDto
	{
		[JsonPropertyName("giasData")]
		public GiasData Gias { get; }

		[JsonConstructor]
		public TrustDetailsDto(GiasData gias) => (Gias) = (gias);

		public class GiasData
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
			public GroupContactAddress GroupContactAddress { get; }
			
			public GiasData(string ukprn, string groupId, string groupName, string companiesHouseNumber, GroupContactAddress groupContactAddress) => 
				(UkPrn, GroupId, GroupName, CompaniesHouseNumber, GroupContactAddress) = (ukprn, groupId, groupName, companiesHouseNumber, groupContactAddress);
		}

		public class GroupContactAddress
		{
			[JsonPropertyName("street")]
			public string Street { get; }
			
			[JsonPropertyName("locality")]
			public string Locality { get; }
			
			[JsonPropertyName("additionalLine")]
			public string AdditionalLine { get; }
			
			[JsonPropertyName("town")]
			public string Town { get; }
			
			[JsonPropertyName("county")]
			public string County { get; }
			
			[JsonPropertyName("postcode")]
			public string Postcode { get; }
			
			public GroupContactAddress(string street, string locality, string additionalLine, string town, string county, string postcode) => 
				(Street, Locality, AdditionalLine, Town, County, Postcode) = (street, locality, additionalLine, town, county, postcode);
		}
	}
}