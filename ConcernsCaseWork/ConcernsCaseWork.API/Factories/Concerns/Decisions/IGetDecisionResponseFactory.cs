using ConcernsCaseWork.API.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;

namespace ConcernsCaseWork.API.Factories.Concerns.Decisions
{
	public interface IGetDecisionResponseFactory
	{
		public GetDecisionResponse Create(int concernsCaseUrn, Decision decision);
	}
}