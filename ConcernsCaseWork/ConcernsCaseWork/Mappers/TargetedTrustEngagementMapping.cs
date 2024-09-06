using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Utils.Extensions;
using System.Linq;

namespace ConcernsCaseWork.Mappers;

public static class TargetedTrustEngagementMapping
{
	public static ActionSummaryModel ToActionSummary(this TargetedTrustEngagementSummaryResponse model)
	{
		var result = new ActionSummaryModel()
		{
			ClosedDate = DateTimeHelper.ParseToDisplayDate(model.ClosedAt),
			Name = $"TTE - {model.Title}",
			OpenedDate = DateTimeHelper.ParseToDisplayDate(model.CreatedAt),
			RelativeUrl = $"/case/{model.CaseUrn}/management/action/targetedtrustengagement/{model.TargetedTrustEngagementId}",
			StatusName = (model.Outcome.HasValue) ? ((TargetedTrustEngagementOutcome)model.Outcome).Description() : "In progress",
			RawOpenedDate = model.CreatedAt,
			RawClosedDate = model.ClosedAt
		};

		return result;
	}

	public static CreateTargetedTrustEngagementRequest ToEditTTEModel(GetTargetedTrustEngagementResponse getTTEResponse)
	{
		var result = new CreateTargetedTrustEngagementRequest()
		{
			CaseUrn = getTTEResponse.CaseUrn,
			EngagementStartDate = getTTEResponse.EngagementStartDate,
			ActivityId = getTTEResponse.ActivityId,
			ActivityTypes = getTTEResponse.ActivityTypes,
			Notes = getTTEResponse.Notes,
			CreatedBy = getTTEResponse.CreatedBy,
		};

		return result;
	}

	public static UpdateTargetedTrustEngagementRequest ToTargetedTrustEngagementRequest(CreateTargetedTrustEngagementRequest createTargetedTrustEngagementRequest)
	{
		var updateTargetedTrustEngagementRequest = new UpdateTargetedTrustEngagementRequest()
		{
			CaseUrn = createTargetedTrustEngagementRequest.CaseUrn,
			EngagementStartDate = createTargetedTrustEngagementRequest.EngagementStartDate,
			ActivityId = createTargetedTrustEngagementRequest.ActivityId,
			ActivityTypes = createTargetedTrustEngagementRequest.ActivityTypes,
			Notes = createTargetedTrustEngagementRequest.Notes
		};

		return updateTargetedTrustEngagementRequest;
	}

	public static ViewTargetedTrustEngagementModel ToViewModel(GetTargetedTrustEngagementResponse response, GetCasePermissionsResponse permissionsResponse)
	{
		var isClosed = response.ClosedAt.HasValue;

		var result = new ViewTargetedTrustEngagementModel()
		{
			Id = response.Id.ToString(),
			IsClosed = response.ClosedAt.HasValue,
			IsEditable = !isClosed && permissionsResponse.HasEditPermissions(),
			EditLink = $"/case/{response.CaseUrn}/management/action/targetedtrustengagement/addOrUpdate/{response.Id}",
			Activity = response.ActivityId.Description(),
			ActivityTypes = response.ActivityTypes.ToList().Select(at => at.Description()).ToList(),
			Outcome = response.EngagementOutcomeId?.Description(),
			Notes = response.Notes,
			DateOpened = DateTimeHelper.ParseToDisplayDate(response.CreatedAt),
			DateClosed = DateTimeHelper.ParseToDisplayDate(response.ClosedAt),
			DateBegan = DateTimeHelper.ParseToDisplayDate(response.EngagementStartDate),
			DateEnded = DateTimeHelper.ParseToDisplayDate(response.EngagementEndDate),
		};

		return result;
	}


}