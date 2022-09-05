using ConcernsCaseWork.Models.CaseActions;
using Service.TRAMS.Nti;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
				ClosedStatusId = ntiModel.ClosedStatusId,
				CreatedAt = ntiModel.CreatedAt,
				Notes = ntiModel.Notes,
				ReasonsMapping = ntiModel.Reasons?.Select(r => r.Id).ToArray(),
				StatusId = ntiModel.Status?.Id,
				DateStarted = ntiModel.DateStarted,
				UpdatedAt = ntiModel.UpdatedAt,
				ConditionsMapping = ntiModel.Conditions?.Select(c => c.Id).ToArray()
			};
		}

		public static NtiModel ToServiceModel(NtiDto ntiDto, ICollection<NtiStatusDto> statuses)
		{
			return new NtiModel
			{
				Id = ntiDto.Id,
				CaseUrn = ntiDto.CaseUrn,
				CreatedAt = ntiDto.CreatedAt,
				UpdatedAt = ntiDto.UpdatedAt ?? default(DateTime),
				Notes = ntiDto.Notes,
				DateStarted = ntiDto.DateStarted,
				Status = ntiDto.StatusId == null ? null : ToServiceModel(statuses.FirstOrDefault(s => s.Id == ntiDto.StatusId.Value)),
				Reasons = ntiDto.ReasonsMapping?.Select(r => new NtiReasonModel { Id = r }).ToArray(),
				Conditions = ntiDto.ConditionsMapping?.Select(c => new NtiConditionModel { Id = c }).ToArray(),
				ClosedStatusId = ntiDto.ClosedStatusId,
				ClosedStatus = ntiDto.ClosedStatusId.HasValue ? new NtiStatusModel { Id = ntiDto.ClosedStatusId.Value } : null,
				ClosedAt = ntiDto.ClosedAt
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

		public static NtiStatusModel ToServiceModel(NtiStatusDto ntiStatusDto)
		{
			return new NtiStatusModel
			{
				Id = ntiStatusDto.Id,
				Name = ntiStatusDto.Name
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
	}
}
