using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models.CaseActions;
using Service.TRAMS.NtiUnderConsideration;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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
				Reasons = ntiModel.NtiReasonsForConsidering?.Select(r => ToDBModel(r)).ToArray(),
				UnderConsiderationReasonsMapping = ntiModel.NtiReasonsForConsidering?.Select(r => r.Id).ToArray(),
				Notes = ntiModel.Notes,
				CreatedAt = ntiModel.CreatedAt,
				UpdatedAt = ntiModel.UpdatedAt,
				ClosedAt = ntiModel.ClosedAt,
				ClosedStatusId = ntiModel.ClosedStatusId,
				ClosedStatusName = ntiModel.ClosedStatusName
			};
		}

		public static NtiUnderConsiderationModel ToServiceModel(NtiUnderConsiderationDto ntiDto)
		{
			return new NtiUnderConsiderationModel
			{
				Id = ntiDto.Id,
				CaseUrn = ntiDto.CaseUrn,
				NtiReasonsForConsidering = ntiDto.Reasons?.Select(r => ToServiceModel(r)).ToArray(),	
				CreatedAt = ntiDto.CreatedAt.Date,
				Notes = ntiDto.Notes,
				UpdatedAt = ntiDto.UpdatedAt,
				ClosedAt = ntiDto.ClosedAt,
				ClosedStatusId = ntiDto.ClosedStatusId,
				ClosedStatusName = ntiDto.ClosedStatusName
			};
		}

		public static NtiUnderConsiderationReasonDto ToDBModel(NtiReasonForConsideringModel ntiReasonModel)
		{
			return new NtiUnderConsiderationReasonDto
			{
				Id = ntiReasonModel.Id,
				Name = ntiReasonModel.Name,
			};
		}

		public static  NtiReasonForConsideringModel ToServiceModel(NtiUnderConsiderationReasonDto ntiDto)
		{
			return new NtiReasonForConsideringModel
			{
				Id = ntiDto.Id,
				Name = ntiDto.Name
			};
		}

		public static ActionSummary ToActionSummary(this NtiUnderConsiderationModel model, IEnumerable<NtiUnderConsiderationStatusDto> statuses)
		{
			var statusName = "In progress";

			if (model.ClosedAt.HasValue)
			{
				statusName = statuses.SingleOrDefault(s => s.Id == model.ClosedStatusId)?.Name ?? string.Empty;
			}

			var result = new ActionSummary
			{
				ClosedDate = model.ClosedAt?.ToDayMonthYear(),
				Name = "NTI Under Consideration",
				OpenedDate = model.CreatedAt.ToDayMonthYear(),
				RelativeUrl = $"/case/{model.CaseUrn}/management/action/ntiunderconsideration/{model.Id}",
				StatusName = statusName
			};

			return result;
		}
	}
}
