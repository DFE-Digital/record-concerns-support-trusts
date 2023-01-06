using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.Status;

namespace ConcernsCaseWork.Mappers
{
	public static class CaseMapping
	{
		public static CreateCaseDto Map(CreateCaseModel createCaseModel)
		{
			return new CreateCaseDto(
				createCaseModel.CreatedAt,
				createCaseModel.UpdatedAt,
				createCaseModel.ReviewAt,
				createCaseModel.CreatedBy,
				createCaseModel.CrmEnquiry,
				createCaseModel.TrustUkPrn,
				createCaseModel.ReasonAtReview,
				createCaseModel.DeEscalation,
				createCaseModel.Issue,
				createCaseModel.CurrentStatus,
				createCaseModel.NextSteps,
				createCaseModel.CaseAim,
				createCaseModel.DeEscalationPoint,
				createCaseModel.CaseHistory,
				createCaseModel.DirectionOfTravel,
				createCaseModel.StatusId,
				createCaseModel.RatingId,
				createCaseModel.Territory);
		}

		public static CaseModel Map(CaseDto caseDto, string status = null)
		{
			return new CaseModel
			{
				CreatedAt = caseDto.CreatedAt,
				UpdatedAt = caseDto.UpdatedAt,
				ReviewAt = caseDto.ReviewAt,
				ClosedAt = caseDto.ClosedAt,
				CreatedBy = caseDto.CreatedBy,
				Description = caseDto.Description,
				CrmEnquiry = caseDto.CrmEnquiry,
				TrustUkPrn = caseDto.TrustUkPrn,
				ReasonAtReview = caseDto.ReasonAtReview,
				DeEscalation = caseDto.DeEscalation,
				Issue = caseDto.Issue,
				CurrentStatus = caseDto.CurrentStatus,
				NextSteps = caseDto.NextSteps,
				CaseAim = caseDto.CaseAim,
				DeEscalationPoint = caseDto.DeEscalationPoint,
				DirectionOfTravel = caseDto.DirectionOfTravel,
				CaseHistory = caseDto.CaseHistory,
				Territory = caseDto.Territory,
				Urn = caseDto.Urn,
				StatusId = caseDto.StatusId,
				StatusName = status,
				RatingId = caseDto.RatingId
			};
		}

		public static CaseDto MapClosure(PatchCaseModel patchCaseModel, CaseDto caseDto, StatusDto statusDto)
			=> caseDto with
			{
				UpdatedAt = patchCaseModel.UpdatedAt, 
				ReviewAt = patchCaseModel.ReviewAt ?? caseDto.ReviewAt,
					ClosedAt = patchCaseModel.ClosedAt ?? caseDto.ClosedAt,
					ReasonAtReview = patchCaseModel.ReasonAtReview,
					StatusId = statusDto.Id
			};

		public static CaseDto MapDirectionOfTravel(PatchCaseModel patchCaseModel, CaseDto caseDto)
			=> caseDto with { UpdatedAt = patchCaseModel.UpdatedAt, DirectionOfTravel = patchCaseModel.DirectionOfTravel };

		public static CaseDto MapIssue(PatchCaseModel patchCaseModel, CaseDto caseDto)
			=> caseDto with { UpdatedAt = patchCaseModel.UpdatedAt, Issue = patchCaseModel.Issue };

		public static CaseDto MapCurrentStatus(PatchCaseModel patchCaseModel, CaseDto caseDto)
			=> caseDto with { UpdatedAt = patchCaseModel.UpdatedAt, CurrentStatus = patchCaseModel.CurrentStatus };

		public static CaseDto MapCaseAim(PatchCaseModel patchCaseModel, CaseDto caseDto)
			=> caseDto with { UpdatedAt = patchCaseModel.UpdatedAt, CaseAim = patchCaseModel.CaseAim };

		public static CaseDto MapDeEscalationPoint(PatchCaseModel patchCaseModel, CaseDto caseDto)
			=> caseDto with { UpdatedAt = patchCaseModel.UpdatedAt, DeEscalationPoint = patchCaseModel.DeEscalationPoint };

		public static CaseDto MapNextSteps(PatchCaseModel patchCaseModel, CaseDto caseDto)
			=> caseDto with { UpdatedAt = patchCaseModel.UpdatedAt, NextSteps = patchCaseModel.NextSteps };

		public static CaseDto MapRating(PatchCaseModel patchCaseModel, CaseDto caseDto)
			=> caseDto with { UpdatedAt = patchCaseModel.UpdatedAt, RatingId = patchCaseModel.RatingId };
	}
}