﻿using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Service.Decision;
using System;
using System.Globalization;
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
			var receivedRequestDate = GetEsfaReceivedRequestDate(decisionResponse);

			var result = new DecisionModel()
			{
				DecisionId = decisionResponse.DecisionId,
				ConcernsCaseUrn = decisionResponse.ConcernsCaseUrn,
				CrmEnquiryNumber = decisionResponse.CrmCaseNumber,
				RetrospectiveApproval = decisionResponse.RetrospectiveApproval != true ? "No" : "Yes",
				SubmissionRequired = decisionResponse.SubmissionRequired != true ? "No" : "Yes",
				SubmissionLink = decisionResponse.SubmissionDocumentLink,
				EsfaReceivedRequestDate = receivedRequestDate?.ToDayMonthYear(),
				TotalAmountRequested = decisionResponse.TotalAmountRequested.ToString("C", new CultureInfo("en-GB")),
				DecisionTypes = decisionResponse.DecisionTypes.Select(d => EnumHelper.GetEnumDescription(d)).ToList(),
				SupportingNotes = decisionResponse.SupportingNotes,
				EditLink = $"/case/{decisionResponse.ConcernsCaseUrn}/management/action/decision/addOrUpdate/{decisionResponse.DecisionId}",
				BackLink = $"/case/{decisionResponse.ConcernsCaseUrn}/management"
			};

			return result;
		}

		public static CreateDecisionRequest ToEditDecisionModel(GetDecisionResponse getDecisionResponse)
		{
			var result = new CreateDecisionRequest()
			{
				ConcernsCaseUrn = getDecisionResponse.ConcernsCaseUrn,
				CrmCaseNumber = getDecisionResponse.CrmCaseNumber,
				DecisionTypes = getDecisionResponse.DecisionTypes,
				ReceivedRequestDate = GetEsfaReceivedRequestDate(getDecisionResponse),
				RetrospectiveApproval = getDecisionResponse.RetrospectiveApproval,
				SubmissionDocumentLink = getDecisionResponse.SubmissionDocumentLink,
				SupportingNotes = getDecisionResponse.SupportingNotes,
				SubmissionRequired = getDecisionResponse.SubmissionRequired,
				TotalAmountRequested = getDecisionResponse.TotalAmountRequested,
			};

			return result;
		}

		public static UpdateDecisionRequest ToUpdateDecision(CreateDecisionRequest createDecisionRequest)
		{
			var updateDecisionRequest = new UpdateDecisionRequest()
			{
				CrmCaseNumber = createDecisionRequest.CrmCaseNumber,
				DecisionTypes = createDecisionRequest.DecisionTypes,
				TotalAmountRequested = createDecisionRequest.TotalAmountRequested,
				SupportingNotes = createDecisionRequest.SupportingNotes,
				ReceivedRequestDate = createDecisionRequest.ReceivedRequestDate,
				SubmissionDocumentLink = createDecisionRequest.SubmissionRequired == true ? createDecisionRequest.SubmissionDocumentLink : null,
				RetrospectiveApproval = createDecisionRequest.RetrospectiveApproval,
				SubmissionRequired = createDecisionRequest.SubmissionRequired
			};

			return updateDecisionRequest;
		}

		public static CreateDecisionOutcomeRequest ToCreateDecisionOutcomeRequest(EditDecisionOutcomeModel model)
		{
			var result = new CreateDecisionOutcomeRequest()
			{
				Status = model.Status,
				TotalAmount = model.TotalAmount,
				Authorizer = model.Authorizer,
				BusinessAreasConsulted = model.BusinessAreasConsulted,
				DecisionMadeDate = ParseDate(model.DecisionMadeDate),
				DecisionEffectiveFromDate = ParseDate(model.DecisionEffectiveFromDate)
			};

			return result;
		}

		public static CreateDecisionOutcomeRequest ToEditDecisionModel(GetDecisionOutcomeResponse getDecisionOutcomeResponse)
		{
			var result = new CreateDecisionOutcomeRequest()
			{
				Status = getDecisionOutcomeResponse.DecisionOutcomeStatus,
				TotalAmount = getDecisionOutcomeResponse.TotalAmountApproved,
				DecisionMadeDate = GetNullableDate(getDecisionOutcomeResponse.DecisionMadeDate),
				DecisionEffectiveFromDate = GetNullableDate(getDecisionOutcomeResponse.DecisionTakeEffectDate),
				Authorizer = getDecisionOutcomeResponse.Authoriser,
				BusinessAreasConsulted = getDecisionOutcomeResponse.BusinessAreasConsulted
			};

			return result;
		}

		private static DateTime? ParseDate(OptionalDateModel date)
		{
			if (date.IsEmpty())
			{
				return null;
			}

			var result = DateTimeHelper.ParseExact(date.ToString());

			return result;
		}

		private static DateTimeOffset? GetNullableDate(DateTimeOffset? date)
		{
			return date.Value != DateTimeOffset.MinValue ? date : null;
		}


		private static DateTimeOffset? GetEsfaReceivedRequestDate(GetDecisionResponse decisionResponse)
		{
			return decisionResponse.ReceivedRequestDate != DateTimeOffset.MinValue ? decisionResponse.ReceivedRequestDate : null;
		}
	}
}
