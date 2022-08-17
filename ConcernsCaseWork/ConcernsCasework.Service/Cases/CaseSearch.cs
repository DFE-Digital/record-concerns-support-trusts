using ConcernsCasework.Service.Base;

namespace ConcernsCasework.Service.Cases
{
	public sealed class CaseSearch : PageSearch
	{
		public long CaseUrn { get; }
		
		public CaseSearch(long caseUrn) => (CaseUrn) = (caseUrn);
	}
}