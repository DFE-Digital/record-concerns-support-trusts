using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Trusts
{
	public class GroupContactAddressDto
	{
		[JsonProperty("street")]
		public virtual string Street { get; set; }
			
		[JsonProperty("locality")]
		public virtual string Locality { get; set; }
			
		[JsonProperty("additionalLine")]
		public virtual string AdditionalLine { get; set; }
			
		[JsonProperty("town")]
		public virtual string Town { get; set; }
			
		[JsonProperty("county")]
		public virtual string County { get; set; }
			
		[JsonProperty("postcode")]
		public virtual string Postcode { get; set; }
			
		[JsonConstructor]
		public GroupContactAddressDto(string street, string locality, string additionalLine, string town, string county, string postcode) => 
			(Street, Locality, AdditionalLine, Town, County, Postcode) = (street, locality, additionalLine, town, county, postcode);
		
		public GroupContactAddressDto() { }
	}
}