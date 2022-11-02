using ConcernsCaseWork.Service.Base;

namespace ConcernsCaseWork.Service.Cases
{
	public sealed class CaseSearch : PageSearch
	{
		public long CaseUrn { get; }
		
		public CaseSearch(long caseUrn) => (CaseUrn) = (caseUrn);
	}
}