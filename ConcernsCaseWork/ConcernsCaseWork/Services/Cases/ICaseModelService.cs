using ConcernsCaseWork.Models;
using Service.Redis.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public interface ICaseModelService
	{
		Task<(IList<HomeModel>, IList<HomeModel>)> GetCasesByCaseworker(string caseworker);
		Task<CaseModel> GetCaseByUrn(string caseworker, long urn);
		Task PatchClosure(PatchCaseModel patchCaseModel);
		Task PatchConcernType(PatchCaseModel patchCaseModel);
		Task PatchRiskRating(PatchCaseModel patchCaseModel);
		Task PatchDirectionOfTravel(PatchCaseModel patchCaseModel);
		Task PatchIssue(PatchCaseModel patchCaseModel);
		Task<CaseModel> PostCase(CreateCaseModel createCaseModel);
	}
}