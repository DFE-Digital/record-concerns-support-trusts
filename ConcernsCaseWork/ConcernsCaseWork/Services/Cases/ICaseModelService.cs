using ConcernsCaseWork.Models;
using Service.Redis.Models;
using Service.TRAMS.Status;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public interface ICaseModelService
	{
		Task<IList<HomeModel>> GetCasesByCaseworkerAndStatus(string caseworker, StatusEnum status);
		Task<CaseModel> GetCaseByUrn(string caseworker, long urn);
		Task PatchClosure(PatchCaseModel patchCaseModel);
		Task PatchConcernType(PatchCaseModel patchCaseModel);
		Task PatchRiskRating(PatchCaseModel patchCaseModel);
		Task PatchDirectionOfTravel(PatchCaseModel patchCaseModel);
		Task PatchIssue(PatchCaseModel patchCaseModel);
		Task PatchCaseAim(PatchCaseModel patchCaseModel);
		Task PatchCurrentStatus(PatchCaseModel patchCaseModel);
		Task PatchDeEscalationPoint(PatchCaseModel patchCaseModel);
		Task PatchNextSteps(PatchCaseModel patchCaseModel);
		Task<CaseModel> PostCase(CreateCaseModel createCaseModel);
	}
}