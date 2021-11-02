namespace Service.TRAMS.Cases
{
	public sealed class CaseTrustSearch : CaseSearch
	{
		public string TrustUkPrn { get; }
		
		public CaseTrustSearch(string trustUkPrn) => (TrustUkPrn) = (trustUkPrn);
	}
}