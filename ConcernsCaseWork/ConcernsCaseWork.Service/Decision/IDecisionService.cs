using ConcernsCaseWork.API.Contracts.Decisions;
using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;

namespace ConcernsCaseWork.Service.Decision
{
	public interface IDecisionService
	{
		Task<CreateDecisionResponse> PostDecision(CreateDecisionRequest createDecisionDto);

		Task<List<DecisionSummaryResponse>> GetDecisionsByCaseUrn(long urn);

		Task<GetDecisionResponse> GetDecision(long urn, int decisionId);

		Task<UpdateDecisionResponse> PutDecision(long caseUrn, long decisionId, UpdateDecisionRequest updateDecisionRequest);

		Task<CreateDecisionOutcomeResponse> PostDecisionOutcome(long caseUrn, long decisionId, CreateDecisionOutcomeRequest createDecisionOutcomeRequest);

		Task<UpdateDecisionOutcomeResponse> PutDecisionOutcome(long caseUrn, long decisionId, UpdateDecisionOutcomeRequest updateDecisionOutcomeRequest);

		Task<CloseDecisionResponse> CloseDecision(int caseUrn, int decisionId, CloseDecisionRequest closeDecisionRequest);
	}
}
