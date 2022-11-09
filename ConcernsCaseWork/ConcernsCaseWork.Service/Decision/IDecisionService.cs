using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;

namespace ConcernsCaseWork.Service.Decision
{
	public interface IDecisionService
	{
		Task<CreateDecisionResponseDto> PostDecision(CreateDecisionRequest createDecisionDto);

		Task<List<DecisionSummaryResponse>> GetDecisionsByCaseUrn(long urn);

		Task<GetDecisionResponse> GetDecision(long urn, int decisionId);
	}
}
