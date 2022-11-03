using ConcernsCaseWork.Service.Base;

namespace ConcernsCaseWork.Service.Cases
{
	public sealed class CaseTrustSearch : PageSearch
	{
		public string TrustUkPrn { get; }
		
		public CaseTrustSearch(string trustUkPrn) => (TrustUkPrn) = (trustUkPrn);
	}
}