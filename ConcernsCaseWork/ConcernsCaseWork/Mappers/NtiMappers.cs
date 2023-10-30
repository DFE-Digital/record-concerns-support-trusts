using ConcernsCaseWork.API.Contracts.NoticeToImprove;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Service.Nti;
using System;
using System.Linq;

namespace ConcernsCaseWork.Mappers
{
	public static class NtiMappers
	{
		public static NtiDto ToDBModel(NtiModel ntiModel)
		{
			return new NtiDto
			{
				Id = ntiModel.Id,
				CaseUrn = ntiModel.CaseUrn,
				ClosedAt = ntiModel.ClosedAt,
				ClosedStatusId = (int?)ntiModel.ClosedStatusId,
				CreatedAt = ntiModel.CreatedAt,
				Notes = ntiModel.Notes,
				ReasonsMapping = ntiModel.Reasons?.Select(r => r.Id).ToArray(),
				StatusId = (int?)ntiModel.Status,
				DateStarted = ntiModel.DateStarted,
				UpdatedAt = ntiModel.UpdatedAt,
				ConditionsMapping = ntiModel.Conditions?.Select(c => c.Id).ToArray(),
				SumissionDecisionId = ntiModel.SubmissionDecisionId,
				DateNTILifted = ntiModel.DateNTILifted,
				DateNTIClosed = ntiModel.DateNTIClosed
			};
		}

		public static NtiModel ToServiceModel(NtiDto ntiDto, GetCasePermissionsResponse permissionResponse)
		{
			var result = ToServiceModel(ntiDto);
			result.IsEditable = permissionResponse.HasEditPermissions() && !ntiDto.ClosedAt.HasValue;

			return result;
		}

		public static NtiModel ToServiceModel(NtiDto ntiDto)
		{
			return new NtiModel
			{
				Id = ntiDto.Id,
				CaseUrn = ntiDto.CaseUrn,
				CreatedAt = ntiDto.CreatedAt,
				UpdatedAt = ntiDto.UpdatedAt ?? default(DateTime),
				Notes = ntiDto.Notes,
				DateStarted = ntiDto.DateStarted,
				Status = (NtiStatus?)ntiDto.StatusId,
				Reasons = ntiDto.ReasonsMapping?.Select(r => new NtiReasonModel { Id = r }).ToArray(),
				Conditions = ntiDto.ConditionsMapping?.Select(c => new NtiConditionModel { Id = c }).ToArray(),
				ClosedStatusId = (NtiStatus?)ntiDto.ClosedStatusId,
				ClosedAt = ntiDto.ClosedAt,
				SubmissionDecisionId = ntiDto.SumissionDecisionId,
				DateNTILifted = ntiDto.DateNTILifted,
				DateNTIClosed = ntiDto.DateNTIClosed
			};
		}

		public static NtiConditionModel ToServiceModel(NtiConditionDto ntiConditionDto)
		{
			return new NtiConditionModel
			{
				Id = ntiConditionDto.Id,
				Name = ntiConditionDto.Name,
				Type = ntiConditionDto.Type == null ? null
				: new NtiConditionTypeModel
				{
					Id = ntiConditionDto.Type.Id,
					Name = ntiConditionDto.Type.Name,
					DisplayOrder = ntiConditionDto.Type.DisplayOrder
				}
			};
		}

		public static NtiReasonModel ToServiceModel(NtiReasonDto ntiReasonDto)
		{
			return new NtiReasonModel
			{
				Id = ntiReasonDto.Id,
				Name = ntiReasonDto.Name
			};
		}

		public static ActionSummaryModel ToActionSummary(this NtiModel model)
		{
			var status = (model.Status != null) ? model.Status.Description() : "In progress";

			if (model.ClosedAt.HasValue)
			{
				status = model.ClosedStatusId?.Description();
			}

			var result = new ActionSummaryModel()
			{
				ClosedDate = DateTimeHelper.ParseToDisplayDate(model.ClosedAt),
				Name = "NTI",
				OpenedDate = DateTimeHelper.ParseToDisplayDate(model.CreatedAt),
				RelativeUrl = $"/case/{model.CaseUrn}/management/action/nti/{model.Id}",
				StatusName = status,
				RawOpenedDate = model.CreatedAt,
				RawClosedDate = model.ClosedAt,
			};

			return result;
		}
	}
}
