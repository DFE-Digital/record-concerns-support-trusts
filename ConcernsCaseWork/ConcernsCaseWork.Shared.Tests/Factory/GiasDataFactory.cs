using ConcernsCaseWork.Models;
using Service.TRAMS.Trusts;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class GiasDataFactory
	{
		public static GiasDataDto BuildGiasDataDto()
		{
			return new GiasDataDto("trust-ukprn", "group-id", "group-name", "Multi-academy trust", "companies-house-number",
				GroupContactAddressFactory.BuildGroupContactAddressDto());
		}
		
		public static GiasDataModel BuildGiasDataModel()
		{
			return new GiasDataModel("ukprn", "group-id", "group-name", "Multi-academy trust", "companies-house-number", 
				GroupContactAddressFactory.BuildGroupContactAddressModel());
		}
	}
}