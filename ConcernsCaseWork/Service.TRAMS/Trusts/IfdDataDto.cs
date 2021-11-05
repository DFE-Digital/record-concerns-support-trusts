using Newtonsoft.Json;

namespace Service.TRAMS.Trusts
{
	public sealed class IfdDataDto
	{
		[JsonProperty("trustType")]
		public string TrustType { get; }
		
		[JsonProperty("trustAddress")]
		public GroupContactAddressDto GroupContactAddress { get; }
		
		[JsonConstructor]
		public IfdDataDto(string trustType, GroupContactAddressDto groupContactAddress) => 
			(TrustType, GroupContactAddress) = (trustType, groupContactAddress);
	}
}