using System.Threading;

namespace Service.TRAMS.RecordSrma
{
	public sealed class TrustSearch
	{
		public string GroupName { get; }
		public string Ukprn { get; }
		public string CompaniesHouseNumber { get; }
		private int _page = 1;
		public int Page { get { return _page; } }
		
		public TrustSearch(string groupName, string ukprn, string companiesHouseNumber) => 
			(GroupName, Ukprn, CompaniesHouseNumber) = (groupName, ukprn, companiesHouseNumber);

		public int PageIncrement()
		{
			return Interlocked.Increment(ref _page);
		}
	}
}