using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.RecordAcademy
{
	public interface ICaseService
	{
		Task<IList<CaseDto>> GetCasesByCaseworker(string caseworker, string statusUrn = "Live");
		Task<CaseDto> GetCaseByUrn(string urn);
		Task<IList<CaseDto>> GetCasesByTrustUkPrn(string trustUkprn);
		Task<IList<CaseDto>> GetCasesByPagination(CaseSearch caseSearch);
		Task<CaseDto> PostCase(CreateCaseDto createCaseDto);
		Task<CaseDto> PatchCaseByUrn(UpdateCaseDto updateCaseDto);
	}
}