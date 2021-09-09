using System.Text.Json.Serialization;

namespace Service.TRAMS.RecordSrma
{
	public sealed class GroupContactAddressDto
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
			
		[JsonConstructor]
		public GroupContactAddressDto(string street, string locality, string additionalLine, string town, string county, string postcode) => 
			(Street, Locality, AdditionalLine, Town, County, Postcode) = (street, locality, additionalLine, town, county, postcode);
	}
}