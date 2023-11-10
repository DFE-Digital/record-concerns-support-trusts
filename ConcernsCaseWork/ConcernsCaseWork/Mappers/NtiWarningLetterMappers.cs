using ConcernsCaseWork.API.Contracts.NtiWarningLetter;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Service.NtiWarningLetter;
using ConcernsCaseWork.Utils.Extensions;
using System;
using System.Linq;

namespace ConcernsCaseWork.Mappers
{
	public static class NtiWarningLetterMappers
	{
		public static NtiWarningLetterDto ToDBModel(NtiWarningLetterModel ntiModel)
		{
			return new NtiWarningLetterDto
			{
				Id = ntiModel.Id,
				CaseUrn = ntiModel.CaseUrn,
				ClosedAt = ntiModel.ClosedAt,
				ClosedStatusId = (int?)ntiModel.ClosedStatusId,
				CreatedAt = ntiModel.CreatedAt,
				Notes = ntiModel.Notes,
				WarningLetterReasonsMapping = ntiModel.Reasons?.Select(r => (int)r).ToArray(),
				StatusId = (int?)ntiModel.Status,
				DateLetterSent = ntiModel.SentDate,
				UpdatedAt = ntiModel.UpdatedAt,
				WarningLetterConditionsMapping = ntiModel.Conditions?.Select(c => c.Id).ToArray()
			};
		}

		public static NtiWarningLetterModel ToServiceModel(NtiWarningLetterDto ntiDto)
		{
			return new NtiWarningLetterModel
			{
				Id = ntiDto.Id,
				CaseUrn = ntiDto.CaseUrn,
				CreatedAt = ntiDto.CreatedAt,
				UpdatedAt = ntiDto.UpdatedAt ?? default(DateTime),
				Notes = ntiDto.Notes,
				SentDate = ntiDto.DateLetterSent,
				Status = (NtiWarningLetterStatus?)ntiDto.StatusId,
				Reasons = ntiDto.WarningLetterReasonsMapping?.Select(r => (NtiWarningLetterReason)r).ToArray(),
				Conditions = ntiDto.WarningLetterConditionsMapping?.Select(c => new NtiWarningLetterConditionModel { Id = c }).ToArray(),
				ClosedStatusId = (NtiWarningLetterStatus?)ntiDto.ClosedStatusId,
				ClosedAt = ntiDto.ClosedAt
			};
		}

		public static NtiWarningLetterModel ToServiceModel(
			NtiWarningLetterDto ntiDto, 
			GetCasePermissionsResponse permissionsResponse)
		{
			var result = ToServiceModel(ntiDto);
			result.IsEditable = permissionsResponse.HasEditPermissions() && !result.ClosedAt.HasValue;

			return result;
		}

		public static NtiWarningLetterConditionModel ToServiceModel(NtiWarningLetterConditionDto ntiConditionDto)
		{
			return new NtiWarningLetterConditionModel
			{
				Id = ntiConditionDto.Id,
				Name = ntiConditionDto.Name,
				Type = ntiConditionDto.Type == null ? null
				: new NtiWarningLetterConditionTypeModel
				{
					Id = ntiConditionDto.Type.Id,
					Name = ntiConditionDto.Type.Name,
					DisplayOrder = ntiConditionDto.Type.DisplayOrder
				}
			};
		}

		public static ActionSummaryModel ToActionSummary(this NtiWarningLetterModel model)
		{
			var status = (model.Status != null) ? model.Status.Description() : "In progress";

			if (model.ClosedAt.HasValue)
			{
				status = GetClosedStatusName(model);
			}

			var result = new ActionSummaryModel()
			{
				ClosedDate = DateTimeHelper.ParseToDisplayDate(model.ClosedAt),
				Name = "NTI Warning Letter",
				OpenedDate = DateTimeHelper.ParseToDisplayDate(model.CreatedAt),
				RelativeUrl = $"/case/{model.CaseUrn}/management/action/ntiwarningletter/{model.Id}",
				StatusName = status,
				RawOpenedDate = model.CreatedAt,
                RawClosedDate = model.ClosedAt
			};

			return result;
		}

		private static string GetClosedStatusName(NtiWarningLetterModel model)
		{
			if (model.ClosedStatusId == NtiWarningLetterStatus.CancelWarningLetter)
			{
				return "Cancelled";
			}

			return model.ClosedStatusId?.Description();
		}
	}
}
