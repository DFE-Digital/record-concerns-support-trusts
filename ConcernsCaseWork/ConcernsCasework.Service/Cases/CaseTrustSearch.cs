using ConcernsCasework.Service.Base;

namespace ConcernsCasework.Service.Cases
{
	public sealed class CaseTrustSearch : PageSearch
	{
		public string TrustUkPrn { get; }
		
		public CaseTrustSearch(string trustUkPrn) => (TrustUkPrn) = (trustUkPrn);
	}
}