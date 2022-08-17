using ConcernsCasework.Service.Base;

namespace ConcernsCasework.Service.Cases
{
	public interface ICaseService
	{
		Task<ApiListWrapper<CaseDto>> GetCasesByCaseworkerAndStatus(CaseCaseWorkerSearch caseCaseWorkerSearch);
		Task<CaseDto> GetCaseByUrn(long urn);
		Task<ApiListWrapper<CaseDto>> GetCasesByTrustUkPrn(CaseTrustSearch caseTrustSearch);
		Task<ApiListWrapper<CaseDto>> GetCases(PageSearch pageSearch);
		Task<CaseDto> PostCase(CreateCaseDto createCaseDto);
		Task<CaseDto> PatchCaseByUrn(CaseDto caseDto);
	}
}