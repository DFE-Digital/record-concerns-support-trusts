using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Models;
using System.Threading.Tasks;
using ConcernsCaseWork.API.Contracts.Case;

namespace ConcernsCaseWork.Services.Cases
{
	public interface ICaseModelService
	{
		Task<CaseModel> GetCaseByUrn(long urn);
		Task PatchClosure(PatchCaseModel patchCaseModel);
		Task PatchCaseRating(PatchCaseModel patchCaseModel);
		Task PatchRecordRating(PatchRecordModel patchRecordModel);
		Task PatchDirectionOfTravel(PatchCaseModel patchCaseModel);
		Task PatchIssue(PatchCaseModel patchCaseModel);
		Task PatchCaseAim(PatchCaseModel patchCaseModel);
		Task PatchCurrentStatus(PatchCaseModel patchCaseModel);
		Task PatchDeEscalationPoint(PatchCaseModel patchCaseModel);
		Task PatchNextSteps(PatchCaseModel patchCaseModel);
		Task PatchCaseHistory(long caseUrn, string userName, string caseHistory);
		Task PatchTerritory(int caseUrn, string userName, Territory? territory);
		Task<long> PostCase(CreateCaseModel createCaseModel);
		Task<long> PatchCase(int caseUrn,CreateCaseModel createCaseModel);
		Task PatchOwner(int caseUrn, string owner);

		Task PatchRegion(int caseUrn, Region? region);
		Task DeleteCase(int caseUrn);
	}
}