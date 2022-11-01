using ConcernsCaseWork.Service.Base;

namespace ConcernsCaseWork.Service.Cases
{
	public sealed class CaseCaseWorkerSearch : PageSearch
	{
		public string CaseWorkerName { get; }
		public long StatusId { get; }

		public CaseCaseWorkerSearch(string caseWorkerName, long statusId) => (CaseWorkerName, StatusId) = (caseWorkerName, statusId);
	}
}