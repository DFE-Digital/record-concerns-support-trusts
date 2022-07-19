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
				UpdatedAt = ntiModel.UpdatedAt
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
				Reasons = ntiDto.Reasons?.Select(r => new NtiWarningLetterReasonModel { Id = r.Id, Name = r.Name }).ToArray()
			};
		}
	}
}
