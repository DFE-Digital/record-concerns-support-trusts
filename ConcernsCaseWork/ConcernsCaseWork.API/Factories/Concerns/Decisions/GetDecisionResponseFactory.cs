using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;

namespace ConcernsCaseWork.API.Factories.Concerns.Decisions
{
	public class GetDecisionResponseFactory : IGetDecisionResponseFactory
	{
		public GetDecisionResponse Create(int concernsCaseUrn, Decision decision)
		{
			_ = concernsCaseUrn <= 0 ? throw new ArgumentOutOfRangeException(nameof(concernsCaseUrn)) : concernsCaseUrn;
			_ = decision ?? throw new ArgumentNullException(nameof(decision));

			return new GetDecisionResponse()
			{
				ConcernsCaseUrn = concernsCaseUrn,
				DecisionId = decision.DecisionId,
				DecisionTypes = decision.DecisionTypes.Select(x => (Contracts.Enums.DecisionType)x.DecisionTypeId).ToArray(),
				TotalAmountRequested = decision.TotalAmountRequested,
				SupportingNotes = decision.SupportingNotes,
				ReceivedRequestDate = decision.ReceivedRequestDate,
				SubmissionDocumentLink = decision.SubmissionDocumentLink,
				SubmissionRequired = decision.SubmissionRequired,
				RetrospectiveApproval = decision.RetrospectiveApproval,
				CrmCaseNumber = decision.CrmCaseNumber,
				CreatedAt = decision.CreatedAt,
				UpdatedAt = decision.UpdatedAt,
				ClosedAt = decision.ClosedAt,
				DecisionStatus = (Contracts.Enums.DecisionStatus)decision.Status,
				Title = decision.GetTitle(),
				Outcome = CreateDecisionOutcome(decision.Outcome)
			};
		}

		private DecisionOutcome  CreateDecisionOutcome(Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome.DecisionOutcome? entity)
		{
			if (entity == null)
			{
				return null;
			}

			var result = new DecisionOutcome()
			{
				DecisionOutcomeId = entity.DecisionOutcomeId,
				Status = entity.Status,
				Authorizer = entity.Authorizer,
				DecisionEffectiveFromDate = entity.DecisionEffectiveFromDate,
				DecisionMadeDate = entity.DecisionMadeDate,
				TotalAmount = entity.TotalAmount,
				BusinessAreasConsulted = entity.BusinessAreasConsulted.Select(b => b.DecisionOutcomeBusinessId).ToList()
		};

		return result;
		}
	}
}