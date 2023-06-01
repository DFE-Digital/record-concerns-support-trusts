using ConcernsCaseWork.Service.Base;

namespace ConcernsCaseWork.Service.Trusts
{
	public sealed class TrustSearch : PageSearch
	{
		public string GroupName { get; set; }
		public string Ukprn { get; set; }
		public string CompaniesHouseNumber { get; set; }

		public TrustSearch()
		{
			
		}

		public TrustSearch(string groupName, string ukprn, string companiesHouseNumber) => 
			(GroupName, Ukprn, CompaniesHouseNumber) = (groupName, ukprn, companiesHouseNumber);
	}
}