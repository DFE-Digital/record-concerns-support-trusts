using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;

namespace ConcernsCaseWork.API.Factories.Concerns.Decisions;

public class CloseDecisionResponseFactory : ICloseDecisionResponseFactory
{
	public CloseDecisionResponse Create(int concernsCaseUrn, int decisionId) =>
		new CloseDecisionResponse(concernsCaseUrn, decisionId);
}