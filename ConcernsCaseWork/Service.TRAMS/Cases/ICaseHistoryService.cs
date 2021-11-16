using Service.TRAMS.Base;
using System.Threading.Tasks;

namespace Service.TRAMS.Cases
{
	public interface ICaseHistoryService
	{
		Task<CaseHistoryDto> PostCaseHistory(CreateCaseHistoryDto createCaseHistoryDto);
		Task<ApiListWrapper<CaseHistoryDto>> GetCasesHistory(CaseSearch caseSearch);
	}
}