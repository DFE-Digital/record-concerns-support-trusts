using ConcernsCaseWork.Models;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.Redis.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
	}
}