namespace ConcernsCaseWork.Service.Decision
{
	public interface IDecisionService
	{
		Task<CreateDecisionResponseDto> PostDecision(CreateDecisionDto createDecisionDto);

		Task<List<DecisionSummaryResponseDto>> GetDecisionsByCaseUrn(long urn);

		Task<GetDecisionResponse> GetDecision(long urn, int decisionId);
	}
}
