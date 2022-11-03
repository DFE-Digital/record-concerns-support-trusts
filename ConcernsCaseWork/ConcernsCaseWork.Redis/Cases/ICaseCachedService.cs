using ConcernsCaseWork.Service.Cases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Cases
{
	public interface ICaseCachedService
	{
		Task<IList<CaseDto>> GetCasesByCaseworkerAndStatus(string caseworker, long statusId);
		Task<CaseDto> GetCaseByUrn(string caseworker, long urn);
		Task<CaseDto> PostCase(CreateCaseDto createCaseDto);
		Task PatchCaseByUrn(CaseDto caseDto);
	}
}