using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Trusts
{
	public class IfdDataDto
	{
		[JsonProperty("trustType")]
		public virtual string TrustType { get; set; }

		[JsonProperty("trustContactPhoneNumber")]
		public virtual string TrustContactPhoneNumber { get; set; }

		[JsonProperty("trustAddress")]
		public virtual GroupContactAddressDto GroupContactAddress { get; set; }
		
		[JsonConstructor]
		public IfdDataDto(string trustType, string trustContactPhoneNumber, GroupContactAddressDto groupContactAddress) => 
			(TrustType, TrustContactPhoneNumber, GroupContactAddress) = (trustType, trustContactPhoneNumber, groupContactAddress);
		
		protected IfdDataDto() { }
	}
}