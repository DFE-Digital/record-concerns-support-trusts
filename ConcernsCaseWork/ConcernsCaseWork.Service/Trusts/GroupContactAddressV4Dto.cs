using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Trusts
{
	public class GroupContactAddressV4Dto
	{
		[JsonProperty("street")]
		public virtual string Street { get; set; }
			
		[JsonProperty("locality")]
		public virtual string Locality { get; set; }
			
		[JsonProperty("additional")]
		public virtual string AdditionalLine { get; set; }
			
		[JsonProperty("town")]
		public virtual string Town { get; set; }
			
		[JsonProperty("county")]
		public virtual string County { get; set; }
			
		[JsonProperty("postcode")]
		public virtual string Postcode { get; set; }
			
		[JsonConstructor]
		public GroupContactAddressV4Dto(string street, string locality, string additionalLine, string town, string county, string postcode) => 
			(Street, Locality, AdditionalLine, Town, County, Postcode) = (street, locality, additionalLine, town, county, postcode);
		
		public GroupContactAddressV4Dto() { }
	}
}