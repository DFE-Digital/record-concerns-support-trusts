using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Service.CaseActions;
using System;

namespace ConcernsCaseWork.Mappers
{
	public static class CaseActionsMapping
	{
		public static SRMAModel Map(SRMADto srmaDto)
		{
			return new SRMAModel
			{
				Id = srmaDto.Id,
				CaseUrn = srmaDto.CaseUrn,
				CreatedAt = srmaDto.CreatedAt,
				ClosedAt = srmaDto.ClosedAt,
				DateOffered = srmaDto.DateOffered,
				DateAccepted = srmaDto.DateAccepted,
				DateReportSentToTrust = srmaDto.DateReportSentToTrust,
				DateVisitEnd = srmaDto.DateVisitEnd,
				DateVisitStart = srmaDto.DateVisitStart,
				Notes = srmaDto.Notes,
				Reason = (Enums.SRMAReasonOffered)(srmaDto.Reason ?? SRMAReasonOffered.Unknown),
				Status = (Enums.SRMAStatus)srmaDto.Status,
				UpdatedAt = srmaDto.UpdatedAt
			};
		}

		public static SRMAModel Map(SRMADto srmaDto, GetCasePermissionsResponse permissionsResponse)
		{
			var result = Map(srmaDto);
			result.IsEditable = permissionsResponse.HasEditPermissions();

			return result;
		}

		public static SRMADto Map(SRMAModel srmaModel)
		{
			return new SRMADto
			{
				Id = Convert.ToInt32(srmaModel.Id),
				CaseUrn = Convert.ToInt32(srmaModel.CaseUrn),
				CreatedAt = srmaModel.CreatedAt,
				CreatedBy = srmaModel.CreatedBy,
				ClosedAt = srmaModel.ClosedAt,
				DateAccepted = srmaModel.DateAccepted,
				DateOffered = srmaModel.DateOffered,
				DateReportSentToTrust = srmaModel.DateReportSentToTrust,
				DateVisitEnd = srmaModel.DateVisitEnd,
				DateVisitStart = srmaModel.DateVisitStart,
				Notes = srmaModel.Notes,
				Reason = (SRMAReasonOffered)srmaModel.Reason,
				Status = (SRMAStatus)srmaModel.Status,
				UpdatedAt = srmaModel.UpdatedAt
			};
		}

		public static ActionSummaryModel ToActionSummary(this SRMAModel srmaModel)
		{
			var relativeUrl = $"/case/{srmaModel.CaseUrn}/management/action/srma/{srmaModel.Id}";

			if (srmaModel.IsClosed)
			{
				relativeUrl += "/closed";
			}

			var result = new ActionSummaryModel()
			{
				ClosedDate = DateTimeHelper.ParseToDisplayDate(srmaModel.ClosedAt),
				Name = "SRMA",
				OpenedDate = DateTimeHelper.ParseToDisplayDate(srmaModel.CreatedAt),
				RelativeUrl = relativeUrl,
				StatusName = EnumHelper.GetEnumDescription(srmaModel.Status),
				RawOpenedDate = srmaModel.CreatedAt,
				RawClosedDate = srmaModel.ClosedAt
			};

			return result;
		}

		public static SrmaCloseTextModel ToSrmaCloseText(string resolution)
		{
			var warningMessageTemplate = "This action cannot be reopened. Check the details are correct, especially dates, before {0}.";
			var buttonHintTemplate = "Do you still want to {0} this SRMA action?";

			switch (resolution)
			{
				case SrmaConstants.ResolutionCancelled:
					return new SrmaCloseTextModel()
					{
						ConfirmText = "Confirm SRMA action was cancelled",
						WarningMessage = string.Format(warningMessageTemplate, "cancelling"),
						Header = "Cancel SRMA action",
						Title = "Cancel SRMA",
						ButtonHint = string.Format(buttonHintTemplate, "cancel"),
						ButtonText = "Cancel SRMA action"
					};

				case SrmaConstants.ResolutionDeclined:
					return new SrmaCloseTextModel()
					{
						ConfirmText = "Confirm SRMA action was declined by trust",
						WarningMessage = string.Format(warningMessageTemplate, "declining"),
						Header = "SRMA action declined",
						Title = "Decline SRMA",
						ButtonHint = string.Format(buttonHintTemplate, "decline"),
						ButtonText = "SRMA action declined"
					};

				case SrmaConstants.ResolutionComplete:
					return new SrmaCloseTextModel()
					{
						ConfirmText = "Confirm SRMA action is complete",
						WarningMessage = string.Format(warningMessageTemplate, "completing"),
						Header = "Complete SRMA action",
						Title = "Complete SRMA",
						ButtonHint = string.Format(buttonHintTemplate, "complete"),
						ButtonText = "Complete SRMA action"
					};

				default:
					throw new Exception($"Unrecognised SRMA resolution status {resolution}");

			}
		}
	}
}
