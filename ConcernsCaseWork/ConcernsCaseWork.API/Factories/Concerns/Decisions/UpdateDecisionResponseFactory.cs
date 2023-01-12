using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;

namespace ConcernsCaseWork.API.Factories.Concerns.Decisions
{
	public class UpdateDecisionResponseFactory : IUpdateDecisionResponseFactory
	{
		public UpdateDecisionResponse Create(int concernsCaseUrn, int decisionId) =>
			new UpdateDecisionResponse(concernsCaseUrn, decisionId);
	}
}
