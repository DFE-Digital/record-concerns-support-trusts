using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Trusts
{
	public sealed class GroupContactAddressDto
	{
		[JsonProperty("street")]
		public string Street { get; }
			
		[JsonProperty("locality")]
		public string Locality { get; }
			
		[JsonProperty("additionalLine")]
		public string AdditionalLine { get; }
			
		[JsonProperty("town")]
		public string Town { get; }
			
		[JsonProperty("county")]
		public string County { get; }
			
		[JsonProperty("postcode")]
		public string Postcode { get; }
			
		[JsonConstructor]
		public GroupContactAddressDto(string street, string locality, string additionalLine, string town, string county, string postcode) => 
			(Street, Locality, AdditionalLine, Town, County, Postcode) = (street, locality, additionalLine, town, county, postcode);
	}
}