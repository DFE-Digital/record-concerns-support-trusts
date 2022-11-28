using ConcernsCaseWork.API.Contracts.Enums;

namespace ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions
{
	public class GetDecisionOutcomeResponse
	{
		public long DecisionId { get; set; }
		public long DecisionOutcomeId { get; set; }
		public DecisionOutcome DecisionOutcome { get; set; }
		public decimal TotalAmountApproved { get; set; }
		public DateTimeOffset? DecisionMadeDate { get; set; }
		public DateTimeOffset? DecisionTakeEffectDate { get; set; }
		public Authoriser Authoriser { get; set; }
		public BusinessArea[] BusinessAreasConsulted { get; set; }
	}
}
