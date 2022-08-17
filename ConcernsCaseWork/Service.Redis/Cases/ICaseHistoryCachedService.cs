using ConcernsCasework.Service.Cases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Cases
{
	public interface ICaseHistoryCachedService
	{
		Task PostCaseHistory(CreateCaseHistoryDto createCaseHistoryDto, string caseworker);
		Task<IList<CaseHistoryDto>> GetCasesHistory(CaseSearch caseSearch, string caseworker);
	}
}