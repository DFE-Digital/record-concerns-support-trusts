namespace ConcernsCaseWork.Service.Decision
{
	public interface IDecisionService
	{
		Task<CreateDecisionResponseDto> PostDecision(CreateDecisionDto createDecisionDto);

		Task<List<GetDecisionResponseDto>> GetDecisionsByCaseUrn(long urn);

		Task<GetDecisionResponseDto> GetDecision(long urn, int decisionId);
	}
}
