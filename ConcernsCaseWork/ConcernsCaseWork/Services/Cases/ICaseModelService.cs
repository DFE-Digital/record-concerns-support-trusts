using ConcernsCaseWork.Models;
using Service.Redis.Models;
using Service.TRAMS.Status;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public interface ICaseModelService
	{
		Task<IList<HomeModel>> GetCasesByCaseworkerAndStatus(IList<string> caseworkers, StatusEnum statusEnum);
		Task<IList<HomeModel>> GetCasesByCaseworkerAndStatus(string caseworker, StatusEnum status);
		Task<CaseModel> GetCaseByUrn(string caseworker, long urn);
		Task<IList<TrustCasesModel>> GetCasesByTrustUkprn(string trustUkprn);
		Task PatchClosure(PatchCaseModel patchCaseModel);
		Task PatchCaseRating(PatchCaseModel patchCaseModel);
		Task PatchRecordRating(PatchRecordModel patchRecordModel);
		Task PatchDirectionOfTravel(PatchCaseModel patchCaseModel);
		Task PatchIssue(PatchCaseModel patchCaseModel);
		Task PatchCaseAim(PatchCaseModel patchCaseModel);
		Task PatchCurrentStatus(PatchCaseModel patchCaseModel);
		Task PatchDeEscalationPoint(PatchCaseModel patchCaseModel);
		Task PatchNextSteps(PatchCaseModel patchCaseModel);
		Task<long> PostCase(CreateCaseModel createCaseModel);
	}
}