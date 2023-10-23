﻿using ConcernsCaseWork.API.Contracts.Decisions;
using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
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
				OpenedDate = DateTimeHelper.ParseToDisplayDate(decisionSummary.CreatedAt),
				ClosedDate = DateTimeHelper.ParseToDisplayDate(decisionSummary.ClosedAt),
				Name = $"Decision: {decisionSummary.Title}",
				StatusName = decisionSummary.Outcome.HasValue ? EnumHelper.GetEnumDescription(decisionSummary.Outcome) : EnumHelper.GetEnumDescription(decisionSummary.Status),
				RelativeUrl = $"/case/{decisionSummary.ConcernsCaseUrn}/management/action/decision/{decisionSummary.DecisionId}",
				RawOpenedDate = decisionSummary.CreatedAt,
				RawClosedDate = decisionSummary.ClosedAt
			};

			return result;
		}

		public static ViewDecisionModel ToViewDecisionModel(
			GetDecisionResponse decisionResponse,
			GetCasePermissionsResponse casePermissionsResponse)
		{
			var receivedRequestDate = GetEsfaReceivedRequestDate(decisionResponse);

			var result = new ViewDecisionModel()
			{
				DecisionId = decisionResponse.DecisionId,
				ConcernsCaseUrn = decisionResponse.ConcernsCaseUrn,
				CrmEnquiryNumber = decisionResponse.CrmCaseNumber,
				HasCrmCase = BoolToString(decisionResponse.HasCrmCase),
				RetrospectiveApproval = BoolToString(decisionResponse.RetrospectiveApproval),
				SubmissionRequired = BoolToString(decisionResponse.SubmissionRequired),
				SubmissionLink = decisionResponse.SubmissionDocumentLink,
				EsfaReceivedRequestDate = DateTimeHelper.ParseToDisplayDate(receivedRequestDate),
				TotalAmountRequested = ToCurrencyField(decisionResponse.TotalAmountRequested),
				DecisionTypes = decisionResponse.DecisionTypes.Select(d => EnumHelper.GetEnumDescription(d.Id)).ToList(),
				SupportingNotes = decisionResponse.SupportingNotes,
				EditLink = $"/case/{decisionResponse.ConcernsCaseUrn}/management/action/decision/addOrUpdate/{decisionResponse.DecisionId}",
				BackLink = $"/case/{decisionResponse.ConcernsCaseUrn}/management",
				Outcome = ToViewDecisionOutcomeModel(decisionResponse),
				IsEditable = decisionResponse.IsEditable && casePermissionsResponse.HasEditPermissions(),
				CreatedDate = DateTimeHelper.ParseToDisplayDate(decisionResponse.CreatedAt),
				ClosedDate = decisionResponse.ClosedAt.HasValue ? DateTimeHelper.ParseToDisplayDate(decisionResponse.ClosedAt.Value) : string.Empty
			};

			return result;
		}

		private static ViewDecisionOutcomeModel ToViewDecisionOutcomeModel(GetDecisionResponse decisionResponse)
		{
			var decisionOutcomeResponse = decisionResponse.Outcome;

			if (decisionOutcomeResponse == null) return null;

			var result = new ViewDecisionOutcomeModel()
			{
				Status = EnumHelper.GetEnumDescription(decisionOutcomeResponse.Status),
				Authorizer = EnumHelper.GetEnumDescription(decisionOutcomeResponse.Authorizer),
				TotalAmount = ToCurrencyField(decisionOutcomeResponse.TotalAmount),
				DecisionMadeDate = DateTimeHelper.ParseToDisplayDate(decisionOutcomeResponse.DecisionMadeDate),
				DecisionEffectiveFromDate = DateTimeHelper.ParseToDisplayDate(decisionOutcomeResponse.DecisionEffectiveFromDate),
				BusinessAreasConsulted = decisionOutcomeResponse.BusinessAreasConsulted.Select(b => EnumHelper.GetEnumDescription(b)).ToList(),
				EditLink = $"/case/{decisionResponse.ConcernsCaseUrn}/management/action/decision/{decisionResponse.DecisionId}/outcome/addOrUpdate/{decisionOutcomeResponse.DecisionOutcomeId}",
			};

			return result;
		}

		public static CreateDecisionRequest ToEditDecisionModel(GetDecisionResponse getDecisionResponse)
		{
			var result = new CreateDecisionRequest()
			{
				ConcernsCaseUrn = getDecisionResponse.ConcernsCaseUrn,
				CrmCaseNumber = getDecisionResponse.CrmCaseNumber,
				HasCrmCase = getDecisionResponse.HasCrmCase,
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
				HasCrmCase = createDecisionRequest.HasCrmCase,
				DecisionTypes = createDecisionRequest.DecisionTypes,
				TotalAmountRequested = createDecisionRequest.TotalAmountRequested,
				SupportingNotes = createDecisionRequest.SupportingNotes,
				ReceivedRequestDate = createDecisionRequest.ReceivedRequestDate,
				SubmissionDocumentLink = createDecisionRequest.SubmissionDocumentLink,
				RetrospectiveApproval = createDecisionRequest.RetrospectiveApproval,
				SubmissionRequired = createDecisionRequest.SubmissionRequired
			};

			return updateDecisionRequest;
		}

		public static UpdateDecisionOutcomeRequest ToUpdateDecisionOutcome(EditDecisionOutcomeModel model)
		{
			var result = new UpdateDecisionOutcomeRequest() {

				Status = model.Status,
				TotalAmount = model.TotalAmount,
				DecisionMadeDate = ParseDate(model.DecisionMadeDate),
				DecisionEffectiveFromDate = ParseDate(model.DecisionEffectiveFromDate),
				Authorizer = model.Authorizer,
				BusinessAreasConsulted = model.BusinessAreasConsulted
			};

			return result;
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

		public static EditDecisionOutcomeModel ToEditDecisionOutcomeModel(GetDecisionResponse decisionResponse)
		{
			var decisionOutcomeResponse = decisionResponse.Outcome;

			if (decisionOutcomeResponse == null) return null;

			var result = new EditDecisionOutcomeModel()
			{
				Status = decisionOutcomeResponse.Status,
				TotalAmount = decisionOutcomeResponse.TotalAmount,
				DecisionMadeDate = ToOptionaDateModel(decisionOutcomeResponse.DecisionMadeDate),
				DecisionEffectiveFromDate = ToOptionaDateModel(decisionOutcomeResponse.DecisionEffectiveFromDate),
				Authorizer = decisionOutcomeResponse.Authorizer,
				BusinessAreasConsulted = decisionOutcomeResponse.BusinessAreasConsulted
			};

			return result;
		}

		public static DecisionSummaryModel ToDecisionSummaryModel(DecisionSummaryResponse apiDecision)
		{
			var result = new DecisionSummaryModel()
			{
				ClosedAt = apiDecision.ClosedAt?.Date
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

		private static OptionalDateModel ToOptionaDateModel(DateTimeOffset? date)
		{
			var result = new OptionalDateModel();

			if (date.HasValue)
			{
				result.Day = date.Value.Day.ToString();
				result.Month = date.Value.Month.ToString();
				result.Year = date.Value.Year.ToString();
			}

			return result;
		}

		private static DateTimeOffset? GetEsfaReceivedRequestDate(GetDecisionResponse decisionResponse)
		{
			return decisionResponse.ReceivedRequestDate != DateTimeOffset.MinValue ? decisionResponse.ReceivedRequestDate : null;
		}

		private static string? ToCurrencyField(decimal? amount)
		{
			return amount?.ToString("C", new CultureInfo("en-GB"));
		}

		private static string? BoolToString(bool? value)
		{
			return value.HasValue ? (value.Value == true ? "Yes" : "No") : null;
		}
	}
}
