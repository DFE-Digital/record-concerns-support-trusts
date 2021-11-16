using Service.TRAMS.Base;

namespace Service.TRAMS.Cases
{
	public sealed class CaseTrustSearch : PageSearch
	{
		public string TrustUkPrn { get; }
		
		public CaseTrustSearch(string trustUkPrn) => (TrustUkPrn) = (trustUkPrn);
	}
}