using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Service.Decision;
using System;
using System.Linq;

namespace ConcernsCaseWork.Services.Decisions
{
	public class DecisionMapping
	{
		public static ActionSummaryModel ToActionSummary(DecisionSummaryResponse decisionSummary)
		{
			var result = new ActionSummaryModel()
			{
				OpenedDate = decisionSummary.CreatedAt.ToDayMonthYear(),
				ClosedDate = decisionSummary.ClosedAt?.ToDayMonthYear(),
				Name = $"Decision: {decisionSummary.Title}",
				StatusName = EnumHelper.GetEnumDescription(decisionSummary.Status),
				RelativeUrl = $"/case/{decisionSummary.ConcernsCaseUrn}/management/action/decision/{decisionSummary.DecisionId}"
			};

			return result;
		}

		public static DecisionModel ToDecisionModel(GetDecisionResponse decisionResponse)
		{
			var result = new DecisionModel()
			{
				DecisionId = decisionResponse.DecisionId,
				ConcernsCaseUrn = decisionResponse.ConcernsCaseUrn,
				CrmEnquiryNumber = decisionResponse.CrmCaseNumber,
				RetrospectiveApproval = decisionResponse.RetrospectiveApproval != true ? "No" : "Yes",
				SubmissionRequired = decisionResponse.SubmissionRequired != true ? "No" : "Yes",
				SubmissionLink = decisionResponse.SubmissionDocumentLink,
				EsfaReceivedRequestDate = GetEsfaReceivedRequestDate(decisionResponse),
				TotalAmountRequested = decisionResponse.TotalAmountRequested.ToString("C"),
				DecisionTypes = decisionResponse.DecisionTypes.Select(d => EnumHelper.GetEnumDescription(d)).ToList(),
				SupportingNotes = decisionResponse.SupportingNotes,
				EditLink = $"/case/{decisionResponse.ConcernsCaseUrn}/management/action/decision/{decisionResponse.DecisionId}/edit",
				BackLink = $"/case/{decisionResponse.ConcernsCaseUrn}/management"
			};

			return result;
		}

		private static string GetEsfaReceivedRequestDate(GetDecisionResponse decisionResponse)
		{
			return decisionResponse.ReceivedRequestDate != DateTimeOffset.MinValue ? decisionResponse.ReceivedRequestDate.ToDayMonthYear() : null;
		}
	}
}
