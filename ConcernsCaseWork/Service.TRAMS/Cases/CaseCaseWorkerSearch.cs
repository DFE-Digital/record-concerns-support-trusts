using Service.TRAMS.Base;

namespace Service.TRAMS.Cases
{
	public sealed class CaseCaseWorkerSearch : PageSearch
	{
		public string CaseWorkerName { get; }
		public long StatusId { get; }

		public CaseCaseWorkerSearch(string caseWorkerName, long statusId) => (CaseWorkerName, StatusId) = (caseWorkerName, statusId);
	}
}