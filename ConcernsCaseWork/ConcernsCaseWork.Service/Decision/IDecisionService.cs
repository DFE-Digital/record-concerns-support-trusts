using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;

namespace ConcernsCaseWork.Service.Decision
{
	public interface IDecisionService
	{
		Task<CreateDecisionResponse> PostDecision(CreateDecisionRequest createDecisionDto);

		Task<List<DecisionSummaryResponse>> GetDecisionsByCaseUrn(long urn);

		Task<GetDecisionResponse> GetDecision(long urn, int decisionId);

		Task<UpdateDecisionResponse> PutDecision(long caseUrn, long decisionId, UpdateDecisionRequest updateDecisionRequest);
		
		Task<CloseDecisionResponse> CloseDecision(long caseUrn, long decisionId, CloseDecisionRequest closeDecisionRequest);
	}
}
