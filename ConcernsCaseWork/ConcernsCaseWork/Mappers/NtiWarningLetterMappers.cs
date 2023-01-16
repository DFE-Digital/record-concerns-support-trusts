using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Service.NtiWarningLetter;
using System;
using System.Collections.Generic;
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
				ClosedStatusId = ntiModel.ClosedStatusId,
				CreatedAt = ntiModel.CreatedAt,
				Notes = ntiModel.Notes,
				WarningLetterReasonsMapping = ntiModel.Reasons?.Select(r => r.Id).ToArray(),
				StatusId = ntiModel.Status?.Id,
				DateLetterSent = ntiModel.SentDate,
				UpdatedAt = ntiModel.UpdatedAt,
				WarningLetterConditionsMapping = ntiModel.Conditions?.Select(c => c.Id).ToArray()
			};
		}

		public static NtiWarningLetterModel ToServiceModel(NtiWarningLetterDto ntiDto, ICollection<NtiWarningLetterStatusDto> statuses)
		{
			return new NtiWarningLetterModel
			{
				Id = ntiDto.Id,
				CaseUrn = ntiDto.CaseUrn,
				CreatedAt = ntiDto.CreatedAt,
				UpdatedAt = ntiDto.UpdatedAt ?? default(DateTime),
				Notes = ntiDto.Notes,
				SentDate = ntiDto.DateLetterSent,
				Status = ntiDto.StatusId == null ? null : ToServiceModel(statuses.FirstOrDefault(s => s.Id == ntiDto.StatusId)),
				Reasons = ntiDto.WarningLetterReasonsMapping?.Select(r => new NtiWarningLetterReasonModel { Id = r }).ToArray(),
				Conditions = ntiDto.WarningLetterConditionsMapping?.Select(c => new NtiWarningLetterConditionModel { Id = c }).ToArray(),
				ClosedStatusId = ntiDto.ClosedStatusId,
				ClosedStatus = ntiDto.ClosedStatusId.HasValue ? ToServiceModel(statuses.FirstOrDefault(s => s.Id == ntiDto.ClosedStatusId)) : null,
				ClosedAt = ntiDto.ClosedAt
			};
		}

		public static NtiWarningLetterModel ToServiceModel(
			NtiWarningLetterDto ntiDto, 
			ICollection<NtiWarningLetterStatusDto> statuses, 
			GetCasePermissionsResponse permissionsResponse)
		{
			var result = ToServiceModel(ntiDto, statuses);
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

		public static NtiWarningLetterStatusModel ToServiceModel(NtiWarningLetterStatusDto ntiStatusDto)
		{
			return new NtiWarningLetterStatusModel
			{
				Id = ntiStatusDto.Id,
				Name = ntiStatusDto.Name,
				PastTenseName = ntiStatusDto.PastTenseName
			};
		}

		public static NtiWarningLetterReasonModel ToServiceModel(NtiWarningLetterReasonDto ntiWarningLetterReasonDto)
		{
			return new NtiWarningLetterReasonModel
			{
				Id = ntiWarningLetterReasonDto.Id,
				Name = ntiWarningLetterReasonDto.Name
			};
		}

		public static ActionSummaryModel ToActionSummary(this NtiWarningLetterModel model)
		{
			var status = (model.Status != null) ? model.Status.Name : "In progress";

			if (model.ClosedAt.HasValue)
			{
				status = model.ClosedStatus.PastTenseName;
			}

			var result = new ActionSummaryModel()
			{
				ClosedDate = model.ClosedAt?.ToDayMonthYear(),
				Name = "NTI Warning Letter",
				OpenedDate = model.CreatedAt.ToDayMonthYear(),
				RelativeUrl = $"/case/{model.CaseUrn}/management/action/ntiwarningletter/{model.Id}",
				StatusName = status,
				RawOpenedDate = model.CreatedAt,
                RawClosedDate = model.ClosedAt
			};

			return result;
		}
	}
}
