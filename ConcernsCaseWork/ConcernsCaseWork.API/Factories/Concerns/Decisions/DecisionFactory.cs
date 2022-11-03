using ConcernsCaseWork.API.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;

namespace ConcernsCaseWork.API.Factories.Concerns.Decisions
{
    public class DecisionFactory : IDecisionFactory 
    {
        public Decision CreateDecision(int concernsCaseId, CreateDecisionRequest request)
        {
            var decisionTypes = request.DecisionTypes.Select(x => new DecisionType(x)).ToArray();

            return Decision.CreateNew(concernsCaseId, request.CrmCaseNumber, request.RetrospectiveApproval,
                request.SubmissionRequired, request.SubmissionDocumentLink, request.ReceivedRequestDate,
                decisionTypes, request.TotalAmountRequested, request.SupportingNotes, DateTimeOffset.Now);
        }
    }
}