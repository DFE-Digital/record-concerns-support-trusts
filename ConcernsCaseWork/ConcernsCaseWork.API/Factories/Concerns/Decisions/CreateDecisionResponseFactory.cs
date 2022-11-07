using ConcernsCaseWork.API.ResponseModels.Concerns.Decisions;

namespace ConcernsCaseWork.API.Factories.Concerns.Decisions
{
	public class CreateDecisionResponseFactory : ICreateDecisionResponseFactory
	{
		public CreateDecisionResponse Create(int concernsCaseUrn, int decisionId) => new CreateDecisionResponse(concernsCaseUrn, decisionId);
	}
}
