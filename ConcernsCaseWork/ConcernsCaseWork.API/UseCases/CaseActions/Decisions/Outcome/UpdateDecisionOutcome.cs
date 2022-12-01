using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome;

namespace ConcernsCaseWork.API.UseCases.CaseActions.Decisions.Outcome
{
	public class UpdateDecisionOutcome : IUseCaseAsync<UpdateDecisionOutcomeUseCaseParams, UpdateDecisionOutcomeResponse>
	{
		private IConcernsCaseGateway _concernsCaseGateway;
		private IDecisionOutcomeGateway _decisionOutcomeGateway;

		public UpdateDecisionOutcome(IConcernsCaseGateway concernsCaseGateway, IDecisionOutcomeGateway decisionOutcomeGateway)
		{
			_concernsCaseGateway = concernsCaseGateway;
			_decisionOutcomeGateway = decisionOutcomeGateway;
		}

		public async Task<UpdateDecisionOutcomeResponse> Execute(UpdateDecisionOutcomeUseCaseParams parameters, CancellationToken cancellationToken)
		{
			var concernsCase = _concernsCaseGateway.GetConcernsCaseByUrn(parameters.ConcernsCaseId);

			if (concernsCase == null) throw new NotFoundException($"Concern with id {parameters.ConcernsCaseId}");

			var decision = concernsCase.Decisions.FirstOrDefault(d => d.DecisionId == parameters.DecisionId);

			if (decision == null) throw new NotFoundException($"Decision with id {parameters.DecisionId}, Case {parameters.ConcernsCaseId}");

			if (decision.Outcome == null) throw new NotFoundException(
				$"Decision with id {parameters.DecisionId} does not have an outcome, Case {parameters.ConcernsCaseId}");

			var model = ToDbModel(parameters.Request, parameters.DecisionId);

			var decisionOutcome = await _decisionOutcomeGateway.UpdateDecisionOutcome(model, cancellationToken);

			return new UpdateDecisionOutcomeResponse()
			{
				DecisionId = parameters.DecisionId,
				ConcernsCaseUrn = parameters.ConcernsCaseId,
				DecisionOutcomeId = decisionOutcome.DecisionOutcomeId
			};
		}

		private Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome.DecisionOutcome ToDbModel(
			UpdateDecisionOutcomeRequest request,
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

	public record UpdateDecisionOutcomeUseCaseParams
	{
		public int ConcernsCaseId { get; set; }
		public int DecisionId { get; set; }
		public UpdateDecisionOutcomeRequest Request { get; set; }
	}
}
