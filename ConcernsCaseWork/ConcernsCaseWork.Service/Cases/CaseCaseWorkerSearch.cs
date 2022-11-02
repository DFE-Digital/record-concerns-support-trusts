using ConcernsCaseWork.Service.Base;

namespace ConcernsCaseWork.Service.Cases
{
	public sealed class CaseCaseWorkerSearch : PageSearch
	{
		public string CaseWorkerName { get; }
		public long StatusUrn { get; }

		public CaseCaseWorkerSearch(string caseWorkerName, long statusUrn) => (CaseWorkerName, StatusUrn) = (caseWorkerName, statusUrn);
	}
}