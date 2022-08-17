using ConcernsCaseWork.Models.CaseActions;
using ConcernsCasework.Service.NtiWarningLetter;
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
				WarningLetterReasonsMapping = ntiModel.Reasons.Select(r => r.Id).ToArray(),
				StatusId = ntiModel.Status?.Id,
				DateLetterSent = ntiModel.SentDate,
				UpdatedAt = ntiModel.UpdatedAt,
				WarningLetterConditionsMapping = ntiModel.Conditions?.Select(c => c.Id).ToArray()
			};
		}

		public static NtiWarningLetterModel ToServiceModel(NtiWarningLetterDto ntiDto, ICollection<NtiWarningLetterStatusDto> statuses )
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
				Name = ntiStatusDto.Name
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
	}
}
