using ConcernsCaseWork.Models;
using Service.TRAMS.Trusts;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class GroupContactAddressFactory
	{
		public static GroupContactAddressDto BuildGroupContactAddressDto()
		{
			return new GroupContactAddressDto("street", "locality", "additional-line", "town", "county", "postcode");
		}
		
		public static GroupContactAddressModel BuildGroupContactAddressModel()
		{
			return new GroupContactAddressModel("street", "locality", "additional-line", "town", "county", "postcode");
		}
	}
}