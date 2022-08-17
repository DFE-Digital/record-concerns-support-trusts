using ConcernsCasework.Service.Base;

namespace ConcernsCasework.Service.Cases
{
	public interface ICaseHistoryService
	{
		Task<CaseHistoryDto> PostCaseHistory(CreateCaseHistoryDto createCaseHistoryDto);
		Task<ApiListWrapper<CaseHistoryDto>> GetCasesHistory(CaseSearch caseSearch);
	}
}