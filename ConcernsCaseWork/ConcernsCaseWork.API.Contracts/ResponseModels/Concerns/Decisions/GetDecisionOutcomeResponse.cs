using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;

namespace ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions
{
	public class GetDecisionOutcomeResponse
	{
		public long DecisionId { get; set; }
		public long DecisionOutcomeId { get; set; }
		public DecisionOutcomeStatus DecisionOutcomeStatus { get; set; }
		public decimal TotalAmountApproved { get; set; }
		public DateTimeOffset? DecisionMadeDate { get; set; }
		public DateTimeOffset? DecisionTakeEffectDate { get; set; }
		public DecisionOutcomeAuthorizer Authoriser { get; set; }
		public List<DecisionOutcomeBusinessArea> BusinessAreasConsulted { get; set; }
	}
}
