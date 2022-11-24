using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome;

namespace ConcernsCaseWork.API.UseCases.CaseActions.Decisions.Outcome
{
	public class CreateDecisionOutcome : IUseCaseAsync<CreateDecisionOutcomeUseCaseParams, CreateDecisionOutcomeResponse>
	{
		private IConcernsCaseGateway _concernsCaseGateway;
		private IDecisionOutcomeGateway _decisionOutcomeGateway;

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

			if (decision == null) throw new NotFoundException($"Decision with id {parameters.DecisionId}");

			var toCreate = ToCreateDbModel(parameters.Request, parameters.DecisionId);
			var decisionOutcome = await _decisionOutcomeGateway.CreateDecisionOutcome(toCreate, cancellationToken);

			return new CreateDecisionOutcomeResponse()
			{
				DecisionId = parameters.DecisionId,
				ConcernsCaseUrn = parameters.ConcernsCaseId,
				DecisionOutcomeId = decisionOutcome.DecisionOutcomeId
			};
		}

		private Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome.DecisionOutcome ToCreateDbModel(
			CreateDecisionOutcomeRequest request,
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

	public record CreateDecisionOutcomeUseCaseParams
	{
		public int ConcernsCaseId { get; set; }
		public int DecisionId { get; set; }
		public CreateDecisionOutcomeRequest Request { get; set; }
	}
}
