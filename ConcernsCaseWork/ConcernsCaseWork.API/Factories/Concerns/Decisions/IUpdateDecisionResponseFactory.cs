using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;

namespace ConcernsCaseWork.API.Factories.Concerns.Decisions
{
	public interface IUpdateDecisionResponseFactory
	{
		public UpdateDecisionResponse Create(int concernsCaseUrn, int decisionId);
	}
}
