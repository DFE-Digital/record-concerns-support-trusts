using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome;
using DecisionOutcome = ConcernsCaseWork.API.Contracts.Decisions.Outcomes.DecisionOutcome;

namespace ConcernsCaseWork.API.Factories.CaseActionFactories
{
	public class DecisionOutcomeFactory
	{
		public static Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome.DecisionOutcome ToDbModel(
			DecisionOutcome request,
			int decisionId)
		{
			var today = DateTimeOffset.Now;

			var result = new Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome.DecisionOutcome()
			{
				DecisionId = decisionId,
				Status = request.Status,
				TotalAmount = request.TotalAmount,
				Authorizer = request.Authorizer,
				CreatedAt = today,
				UpdatedAt = today,
				DecisionMadeDate = request.DecisionMadeDate,
				DecisionEffectiveFromDate = request.DecisionEffectiveFromDate,
				BusinessAreasConsulted = request.BusinessAreasConsulted.Select(b => new DecisionOutcomeBusinessAreaMapping()
				{
					DecisionOutcomeBusinessId = b
				}).ToList()
			};

			return result;
		}
	}
}
