using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;

namespace ConcernsCaseWork.API.Factories.Concerns.Decisions
{
	public interface ICreateDecisionResponseFactory
	{
		public CreateDecisionResponse Create(int concernsCaseUrn, int decisionId);
	}
}