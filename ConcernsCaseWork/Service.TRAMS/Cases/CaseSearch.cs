using Service.TRAMS.Base;

namespace Service.TRAMS.Cases
{
	public sealed class CaseSearch : PageSearch
	{
		public long CaseUrn { get; }
		
		public CaseSearch(long caseUrn) => (CaseUrn) = (caseUrn);
	}
}