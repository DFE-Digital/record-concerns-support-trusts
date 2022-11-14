using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;

namespace ConcernsCaseWork.API.Factories.Concerns.Decisions
{
	public class GetDecisionsSummariesFactory : IGetDecisionsSummariesFactory
	{
		public DecisionSummaryResponse[] Create(int concernsCaseUrn, IEnumerable<Decision> decisions)
		{
			_ = concernsCaseUrn > 0 ? concernsCaseUrn : throw new ArgumentOutOfRangeException(nameof(concernsCaseUrn));
			_ = decisions ?? throw new ArgumentNullException(nameof(decisions));

			return decisions.Select(decision => new DecisionSummaryResponse()
			{
				ConcernsCaseUrn = concernsCaseUrn,
				DecisionId = decision.DecisionId,
				DecisionStatus = (Contracts.Enums.DecisionStatus)decision.Status,
				CreatedAt = decision.CreatedAt,
				UpdatedAt = decision.UpdatedAt,
				ClosedAt = decision.ClosedAt,
				Title = decision.GetTitle(),
			}).ToArray();
		}
	}
}
