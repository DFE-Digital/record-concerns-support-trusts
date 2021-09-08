using Service.TRAMS.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Cases
{
	public interface ICaseService
	{
		Task<IList<CaseDto>> GetCasesByCaseworker(string caseworker);
		Task<CaseDto> GetCasesByUrn(string urn);
		Task<IList<CaseDto>> GetCasesByTrustUkPrn(string trustUkprn);
		Task<IList<CaseDto>> GetCasesByPagination(CaseSearch caseSearch);
		Task<CaseDto> PostCase(CaseDto caseDto);
		Task<CaseDto> PatchCaseByUrn(CaseDto caseDto);
	}
}