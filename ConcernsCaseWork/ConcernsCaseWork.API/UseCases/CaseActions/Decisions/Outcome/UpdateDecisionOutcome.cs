using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases.CaseActions.Decisions.Outcome
{
	public class UpdateDecisionOutcome : IUseCaseAsync<UpdateDecisionOutcomeUseCaseParams, UpdateDecisionOutcomeResponse>
	{
		private readonly IConcernsCaseGateway _concernsCaseGateway;
		private readonly IDecisionOutcomeGateway _decisionOutcomeGateway;

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

			var model = DecisionOutcomeFactory.ToDbModel(parameters.Request, parameters.DecisionId);

			var decisionOutcome = await _decisionOutcomeGateway.UpdateDecisionOutcome(model, cancellationToken);

			return new UpdateDecisionOutcomeResponse()
			{
				DecisionId = parameters.DecisionId,
				ConcernsCaseUrn = parameters.ConcernsCaseId,
				DecisionOutcomeId = decisionOutcome.DecisionOutcomeId
			};
		}
	}

	public record UpdateDecisionOutcomeUseCaseParams
	{
		public int ConcernsCaseId { get; set; }
		public int DecisionId { get; set; }
		public UpdateDecisionOutcomeRequest Request { get; set; }
	}
}
