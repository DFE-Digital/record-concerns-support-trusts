using ConcernsCaseWork.Models;
using Service.TRAMS.RecordAcademy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public interface ICaseModelService
	{
		Task<(IList<HomeModel>, IList<HomeModel>)> GetCasesByCaseworker(string caseworker);
		Task<HomeModel> GetCasesByUrn(string urn);
		Task<IList<HomeModel>> GetCasesByTrustUkPrn(string trustUkprn);
		Task<IList<HomeModel>> GetCasesByPagination(CaseSearch caseSearch);
		Task<HomeModel> PostCase(HomeModel homeModel);
		Task<HomeModel> PatchCaseByUrn(HomeModel homeModel);
	}
}