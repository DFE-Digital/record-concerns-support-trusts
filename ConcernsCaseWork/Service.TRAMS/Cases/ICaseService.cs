using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Cases
{
	public interface ICaseService
	{
		Task<IList<CaseDto>> GetCasesByCaseworker(string caseworker, string statusName = "Live");
		Task<CaseDto> GetCaseByUrn(long urn);
		Task<IList<CaseDto>> GetCasesByTrustUkPrn(string trustUkprn);
		Task<IList<CaseDto>> GetCasesByPagination(CaseSearch caseSearch);
		Task<CaseDto> PostCase(CreateCaseDto createCaseDto);
		Task<CaseDto> PatchCaseByUrn(CaseDto caseDto);
	}
}