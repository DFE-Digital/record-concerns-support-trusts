using Newtonsoft.Json;

namespace ConcernsCasework.Service.Trusts
{
	public sealed class IfdDataDto
	{
		[JsonProperty("trustType")]
		public string TrustType { get; }

		[JsonProperty("trustContactPhoneNumber")]
		public string TrustContactPhoneNumber { get; }

		[JsonProperty("trustAddress")]
		public GroupContactAddressDto GroupContactAddress { get; }
		
		[JsonConstructor]
		public IfdDataDto(string trustType, string trustContactPhoneNumber, GroupContactAddressDto groupContactAddress) => 
			(TrustType, TrustContactPhoneNumber, GroupContactAddress) = (trustType, trustContactPhoneNumber, groupContactAddress);
	}
}