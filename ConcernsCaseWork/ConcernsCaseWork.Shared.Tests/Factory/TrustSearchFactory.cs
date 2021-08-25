using Service.TRAMS.Models;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class TrustSearchFactory
	{
		public static TrustSearch CreateTrustSearch(string groupName = "", string ukprn = "", string companiesHouseNumber = "")
		{
			return new TrustSearch(groupName, ukprn, companiesHouseNumber);
		}
	}
}