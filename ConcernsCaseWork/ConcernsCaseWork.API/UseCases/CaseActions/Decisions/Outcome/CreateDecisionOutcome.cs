using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome;

namespace ConcernsCaseWork.API.UseCases.CaseActions.Decisions.Outcome
{
	public class CreateDecisionOutcome : IUseCaseAsync<CreateDecisionOutcomeUseCaseParams, CreateDecisionOutcomeResponse>
	{
		private readonly IConcernsCaseGateway _concernsCaseGateway;
		private readonly IDecisionOutcomeGateway _decisionOutcomeGateway;

		public CreateDecisionOutcome(IConcernsCaseGateway concernsCaseGateway, IDecisionOutcomeGateway decisionOutcomeGateway)
		{
			_concernsCaseGateway = concernsCaseGateway;
			_decisionOutcomeGateway = decisionOutcomeGateway;
		}

		public async Task<CreateDecisionOutcomeResponse> Execute(CreateDecisionOutcomeUseCaseParams parameters, CancellationToken cancellationToken)
		{
			var concernsCase = _concernsCaseGateway.GetConcernsCaseByUrn(parameters.ConcernsCaseId);

			if (concernsCase == null) throw new NotFoundException($"Concern with id {parameters.ConcernsCaseId}");

			var decision = concernsCase.Decisions.FirstOrDefault(d => d.DecisionId == parameters.DecisionId);

			if (decision == null) throw new NotFoundException($"Decision with id {parameters.DecisionId}, Case {parameters.ConcernsCaseId}");

			if (decision.Outcome != null) throw new ResourceConflictException(
				$"Decision with id {parameters.DecisionId} already has an outcome, Case {parameters.ConcernsCaseId}");

			var toCreate = DecisionOutcomeFactory.ToDbModel(parameters.Request, parameters.DecisionId);
			var decisionOutcome = await _decisionOutcomeGateway.CreateDecisionOutcome(toCreate, cancellationToken);

			return new CreateDecisionOutcomeResponse()
			{
				DecisionId = parameters.DecisionId,
				ConcernsCaseUrn = parameters.ConcernsCaseId,
				DecisionOutcomeId = decisionOutcome.DecisionOutcomeId
			};
		}
	}

	public record CreateDecisionOutcomeUseCaseParams
	{
		public int ConcernsCaseId { get; set; }
		public int DecisionId { get; set; }
		public CreateDecisionOutcomeRequest Request { get; set; }
	}
}
