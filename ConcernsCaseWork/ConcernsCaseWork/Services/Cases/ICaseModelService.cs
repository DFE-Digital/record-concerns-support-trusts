using ConcernsCaseWork.Models;
using Service.TRAMS.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public interface ICaseModelService
	{
		Task<(IList<HomeUiModel>, IList<HomeUiModel>)> GetCasesByCaseworker(string caseworker);
		Task<HomeUiModel> GetCasesByUrn(string urn);
		Task<IList<HomeUiModel>> GetCasesByTrustUkPrn(string trustUkprn);
		Task<IList<HomeUiModel>> GetCasesByPagination(CaseSearch caseSearch);
		Task<HomeUiModel> PostCase(HomeUiModel caseDto);
		Task<HomeUiModel> PatchCaseByUrn(HomeUiModel caseDto);
	}
}