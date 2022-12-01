using Ardalis.GuardClauses;

namespace ConcernsCaseWork.API.UseCases.CaseActions.Decisions;

public class DecisionUseCaseRequestWrapper<T>
{
	public int CaseUrn { get; set; }
	public int? DecisionId { get; set; }
	public T Request { get; set; }
	
	public DecisionUseCaseRequestWrapper(int caseUrn, int? decisionId, T request)
	{
		Guard.Against.Null(caseUrn);
		Guard.Against.Null(request);
		
		CaseUrn = caseUrn;
		DecisionId = decisionId;
		Request = request;
	}
}