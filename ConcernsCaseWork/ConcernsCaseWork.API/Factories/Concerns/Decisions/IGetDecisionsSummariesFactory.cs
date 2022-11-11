using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;

namespace ConcernsCaseWork.API.Factories.Concerns.Decisions
{
	public interface IGetDecisionsSummariesFactory
	{
		public DecisionSummaryResponse[] Create(int concernsCaseUrn, IEnumerable<Decision> decisions);
	}
}