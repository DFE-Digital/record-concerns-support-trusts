using ConcernsCaseWork.API.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;

namespace ConcernsCaseWork.API.Factories.Concerns.Decisions
{
	public class DecisionFactory : IDecisionFactory
	{
		public Decision CreateDecision(CreateDecisionRequest request)
		{
			_ = request ?? throw new ArgumentNullException(nameof(request));

			var decisionTypes = request.DecisionTypes.Select(x => new DecisionType(x)).Distinct().ToArray();

			return Decision.CreateNew(request.CrmCaseNumber, request.RetrospectiveApproval,
				request.SubmissionRequired, request.SubmissionDocumentLink, request.ReceivedRequestDate,
				decisionTypes, request.TotalAmountRequested, request.SupportingNotes, DateTimeOffset.Now);
		}

		public Decision CreateDecision(UpdateDecisionRequest request)
		{
			_ = request ?? throw new ArgumentNullException(nameof(request));

			var decisionTypes = request.DecisionTypes.Select(x => new DecisionType(x)).Distinct().ToArray();

			return Decision.CreateNew(request.CrmCaseNumber, request.RetrospectiveApproval,
				request.SubmissionRequired, request.SubmissionDocumentLink, request.ReceivedRequestDate,
				decisionTypes, request.TotalAmountRequested, request.SupportingNotes, DateTimeOffset.Now);
		}
	}
}