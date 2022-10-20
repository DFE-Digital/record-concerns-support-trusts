using ConcernsCaseWork.API.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;

namespace ConcernsCaseWork.API.Factories.Concerns.Decisions
{
    public class GetDecisionResponseFactory : IGetDecisionResponseFactory
    {
        public GetDecisionResponse Create(Decision decision)
        {
            _ = decision ?? throw new ArgumentNullException(nameof(decision));
            
            return new GetDecisionResponse()
            {
                ConcernsCaseId = decision.ConcernsCaseId,
                DecisionId = decision.DecisionId,
                DecisionTypes = decision.DecisionTypes.Select(x => x.DecisionTypeId).ToArray(),
                TotalAmountRequested = decision.TotalAmountRequested,
                SupportingNotes =decision.SupportingNotes,
                ReceivedRequestDate = decision.ReceivedRequestDate,
                SubmissionDocumentLink = decision.SubmissionDocumentLink,
                SubmissionRequired = decision.SubmissionRequired,
                RetrospectiveApproval = decision.RetrospectiveApproval,
                CrmCaseNumber = decision.CrmCaseNumber,
                CreatedAt = decision.CreatedAt,
                UpdatedAt = decision.UpdatedAt
            };
        }
    }
}