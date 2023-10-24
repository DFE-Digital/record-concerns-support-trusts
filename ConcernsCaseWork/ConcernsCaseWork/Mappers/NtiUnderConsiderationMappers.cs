using ConcernsCaseWork.API.Contracts.NtiUnderConsideration;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Service.NtiUnderConsideration;
using System.Linq;

namespace ConcernsCaseWork.Mappers
{
	public static class NtiUnderConsiderationMappers
	{
		public static NtiUnderConsiderationDto ToDBModel(NtiUnderConsiderationModel ntiModel)
		{
			return new NtiUnderConsiderationDto
			{
				Id = ntiModel.Id,
				CaseUrn = ntiModel.CaseUrn,
				UnderConsiderationReasonsMapping = ntiModel.NtiReasonsForConsidering?.Select(r => (int)r).ToArray(),
				Notes = ntiModel.Notes,
				CreatedAt = ntiModel.CreatedAt,
				UpdatedAt = ntiModel.UpdatedAt,
				ClosedAt = ntiModel.ClosedAt,
				ClosedStatusId = (int)ntiModel.ClosedStatusId,
			};
		}

		public static NtiUnderConsiderationModel ToServiceModel(NtiUnderConsiderationDto ntiDto)
		{
			var result = new NtiUnderConsiderationModel
			{
				Id = ntiDto.Id,
				CaseUrn = ntiDto.CaseUrn,
				NtiReasonsForConsidering = ntiDto.UnderConsiderationReasonsMapping?.Select(r => (NtiUnderConsiderationReason)r).ToArray(),	
				CreatedAt = ntiDto.CreatedAt,
				Notes = ntiDto.Notes,
				UpdatedAt = ntiDto.UpdatedAt,
				ClosedAt = ntiDto.ClosedAt,
				ClosedStatusId = (NtiUnderConsiderationClosedStatus?) ntiDto.ClosedStatusId,
			};

			if (result.ClosedAt.HasValue)
			{
				result.ClosedStatusName = result.ClosedStatusId?.Description();
			}

			return result;
		}

		public static NtiUnderConsiderationModel ToServiceModel(NtiUnderConsiderationDto ntiDto, GetCasePermissionsResponse permissionsResponse)
		{
			var result = ToServiceModel(ntiDto);
			result.IsEditable = permissionsResponse.HasEditPermissions() && !result.ClosedAt.HasValue;

			return result;
		}

		public static ActionSummaryModel ToActionSummary(this NtiUnderConsiderationModel model)
		{
			var statusName = "In progress";

			if (model.ClosedAt.HasValue)
			{
				statusName = model.ClosedStatusId?.Description();
			}

			var result = new ActionSummaryModel
			{
				ClosedDate = DateTimeHelper.ParseToDisplayDate(model.ClosedAt),
				Name = "NTI Under Consideration",
				OpenedDate = DateTimeHelper.ParseToDisplayDate(model.CreatedAt),
				RelativeUrl = $"/case/{model.CaseUrn}/management/action/ntiunderconsideration/{model.Id}",
				StatusName = statusName,
				RawOpenedDate = model.CreatedAt,
				RawClosedDate = model.ClosedAt
			};

			return result;
		}
	}
}
