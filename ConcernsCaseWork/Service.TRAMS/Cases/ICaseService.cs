using Service.TRAMS.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Cases
{
	public interface ICaseService
	{
		Task<IList<CaseDto>> GetCasesByCaseworkerAndStatus(string caseworker, long statusUrn);
		Task<CaseDto> GetCaseByUrn(long urn);
		Task<ApiWrapper<CaseDto>> GetCasesByTrustUkPrn(CaseTrustSearch caseTrustSearch);
		Task<IList<CaseDto>> GetCasesByPagination(CaseSearch caseSearch);
		Task<CaseDto> PostCase(CreateCaseDto createCaseDto);
		Task<CaseDto> PatchCaseByUrn(CaseDto caseDto);
	}
}