using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Cases
{
	public interface ICaseCachedService
	{
		Task<IList<CaseDto>> GetCasesByCaseworker(string caseworker, string statusUrn = "Live");
		Task<CaseDto> GetCaseByUrn(string caseworker, long urn);
		Task<CaseDto> PostCase(CreateCaseDto createCaseDto);
		Task PatchCaseByUrn(CaseDto caseDto);
		Task<Boolean> IsCasePrimary(string caseworker, long caseUrn);
	}
}