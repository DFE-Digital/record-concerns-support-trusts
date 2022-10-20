using ConcernsCaseWork.Service.Base;

namespace ConcernsCaseWork.Service.Cases
{
	public interface ICaseHistoryService
	{
		Task<CaseHistoryDto> PostCaseHistory(CreateCaseHistoryDto createCaseHistoryDto);
		Task<ApiListWrapper<CaseHistoryDto>> GetCasesHistory(CaseSearch caseSearch);
	}
}