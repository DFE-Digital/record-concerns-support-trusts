namespace ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;

public class CloseDecisionResponse
{
	public CloseDecisionResponse()
	{

	}
	public CloseDecisionResponse(int caseUrn, int decisionId)
	{
		CaseUrn = caseUrn >= 0 ? caseUrn : throw new ArgumentOutOfRangeException(nameof(caseUrn), "value must be greater than zero");
		DecisionId = decisionId >= 0 ? decisionId : throw new ArgumentOutOfRangeException(nameof(decisionId), "value must be greater than zero"); ;
	}

	public int CaseUrn { get; set; }
	public int DecisionId { get; set; }
}