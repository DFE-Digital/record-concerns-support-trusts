using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;

namespace ConcernsCaseWork.API.Factories.Concerns.Decisions
{
	public interface IDecisionFactory
	{
		public Decision CreateDecision(CreateDecisionRequest request);
		public Decision CreateDecision(UpdateDecisionRequest request);
	}
}
