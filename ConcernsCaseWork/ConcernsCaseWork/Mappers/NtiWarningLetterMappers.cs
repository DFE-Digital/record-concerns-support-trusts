using ConcernsCaseWork.Models.CaseActions;
using Service.TRAMS.NtiWarningLetter;
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
				CreatedAt = ntiModel.CreatedAt,
				Notes = ntiModel.Notes,
				Reasons = ntiModel.Reasons.Select(r => new NtiWarningLetterReasonDto { Id = r.Id, Name = r.Name }).ToArray(),
				Status = ntiModel.Status == null ? null : new NtiWarningLetterStatusDto { Id = ntiModel.Status.Id, Name = ntiModel.Status.Name },
				SentDate = ntiModel.SentDate,
				UpdatedAt = ntiModel.UpdatedAt,
				Conditions = ntiModel.Conditions?.Select(c => ToDbModel(c)).ToArray()
			};
		}

		public static NtiWarningLetterModel ToServiceModel(NtiWarningLetterDto ntiDto)
		{
			return new NtiWarningLetterModel
			{
				Id = ntiDto.Id,
				CaseUrn = ntiDto.CaseUrn,
				CreatedAt = ntiDto.CreatedAt,
				UpdatedAt = ntiDto.UpdatedAt,
				Notes = ntiDto.Notes,
				SentDate = ntiDto.SentDate,
				Status = ntiDto.Status == null ? null : new NtiWarningLetterStatusModel { Id = ntiDto.Status.Id, Name = ntiDto.Status.Name },
				Reasons = ntiDto.Reasons?.Select(r => new NtiWarningLetterReasonModel { Id = r.Id, Name = r.Name }).ToArray(),
				Conditions = ntiDto.Conditions?.Select(c => ToServiceModel(c)).ToArray()
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

		public static NtiWarningLetterConditionDto ToDbModel(NtiWarningLetterConditionModel serviceModel)
		{
			return new NtiWarningLetterConditionDto
			{
				Id = serviceModel.Id,
				Name = serviceModel.Name,
				Type = serviceModel.Type == null ? null
				: new NtiWarningLetterConditionTypeDto
				{
					Id = serviceModel.Type.Id,
					Name = serviceModel.Type.Name,
					DisplayOrder = serviceModel.Type.DisplayOrder
				}
			};
		}
	}
}
