using ConcernsCaseWork.Models;
using Service.Redis.Models;
using Service.TRAMS.Cases;
using Service.TRAMS.Status;

namespace ConcernsCaseWork.Mappers
{
	public static class CaseMapping
	{
		public static CreateCaseDto Map(CreateCaseModel createCaseModel)
		{
			return new CreateCaseDto(createCaseModel.CreatedAt, createCaseModel.UpdatedAt, createCaseModel.ReviewAt, createCaseModel.ClosedAt, 
				createCaseModel.CreatedBy, createCaseModel.Description, createCaseModel.CrmEnquiry, createCaseModel.TrustUkPrn, createCaseModel.ReasonAtReview,
				createCaseModel.DeEscalation, createCaseModel.Issue, createCaseModel.CurrentStatus, createCaseModel.NextSteps, createCaseModel.CaseAim,
				createCaseModel.DeEscalationPoint, createCaseModel.DirectionOfTravel, createCaseModel.Urn, createCaseModel.Status);
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
				Urn = caseDto.Urn,
				Status = caseDto.Status,
				StatusName = status
			};
		}

		public static CaseDto Map(PatchCaseModel patchCaseModel, CaseDto caseDto)
		{
			return new CaseDto(caseDto.CreatedAt, patchCaseModel.UpdatedAt,
				caseDto.ReviewAt, caseDto.ClosedAt, caseDto.CreatedBy, caseDto.Description,
				caseDto.CrmEnquiry, caseDto.TrustUkPrn, caseDto.ReasonAtReview,
				caseDto.DeEscalation, caseDto.Issue, caseDto.CurrentStatus,
				caseDto.NextSteps, caseDto.CaseAim, caseDto.DeEscalationPoint, caseDto.DirectionOfTravel,
				caseDto.Urn, caseDto.Status);
		}
		
		public static CaseDto MapClosure(PatchCaseModel patchCaseModel, CaseDto caseDto, StatusDto statusDto)
		{
			return new CaseDto(caseDto.CreatedAt, patchCaseModel.UpdatedAt,
				patchCaseModel.ReviewAt ?? caseDto.ReviewAt, 
				patchCaseModel.ClosedAt ?? caseDto.ClosedAt, 
				caseDto.CreatedBy, caseDto.Description,
				caseDto.CrmEnquiry, caseDto.TrustUkPrn, patchCaseModel.ReasonAtReview,
				caseDto.DeEscalation, caseDto.Issue, caseDto.CurrentStatus,
				caseDto.NextSteps, caseDto.CaseAim, caseDto.DeEscalationPoint, caseDto.DirectionOfTravel,
				caseDto.Urn, statusDto.Urn);
		}
		
		public static CaseDto MapDirectionOfTravel(PatchCaseModel patchCaseModel, CaseDto caseDto)
		{
			return new CaseDto(caseDto.CreatedAt, patchCaseModel.UpdatedAt,
				caseDto.ReviewAt, caseDto.ClosedAt, caseDto.CreatedBy, caseDto.Description,
				caseDto.CrmEnquiry, caseDto.TrustUkPrn, caseDto.ReasonAtReview,
				caseDto.DeEscalation, caseDto.Issue, caseDto.CurrentStatus,
				caseDto.NextSteps, caseDto.CaseAim, caseDto.DeEscalationPoint, patchCaseModel.DirectionOfTravel,
				caseDto.Urn, caseDto.Status);
		}
		
		public static CaseDto MapIssue(PatchCaseModel patchCaseModel, CaseDto caseDto)
		{
			return new CaseDto(caseDto.CreatedAt, patchCaseModel.UpdatedAt,
				caseDto.ReviewAt, caseDto.ClosedAt, caseDto.CreatedBy, caseDto.Description,
				caseDto.CrmEnquiry, caseDto.TrustUkPrn, caseDto.ReasonAtReview,
				caseDto.DeEscalation, patchCaseModel.Issue, caseDto.CurrentStatus,
				caseDto.NextSteps, caseDto.CaseAim, caseDto.DeEscalationPoint, caseDto.DirectionOfTravel,
				caseDto.Urn, caseDto.Status);
		}
		
		public static CaseDto MapCurrentStatus(PatchCaseModel patchCaseModel, CaseDto caseDto)
		{
			return new CaseDto(caseDto.CreatedAt, patchCaseModel.UpdatedAt,
				caseDto.ReviewAt, caseDto.ClosedAt, caseDto.CreatedBy, caseDto.Description,
				caseDto.CrmEnquiry, caseDto.TrustUkPrn, caseDto.ReasonAtReview,
				caseDto.DeEscalation, caseDto.Issue, patchCaseModel.CurrentStatus,
				caseDto.NextSteps, caseDto.CaseAim, caseDto.DeEscalationPoint, caseDto.DirectionOfTravel,
				caseDto.Urn, caseDto.Status);
		}
		
		public static CaseDto MapCaseAim(PatchCaseModel patchCaseModel, CaseDto caseDto)
		{
			return new CaseDto(caseDto.CreatedAt, patchCaseModel.UpdatedAt,
				caseDto.ReviewAt, caseDto.ClosedAt, caseDto.CreatedBy, caseDto.Description,
				caseDto.CrmEnquiry, caseDto.TrustUkPrn, caseDto.ReasonAtReview,
				caseDto.DeEscalation, caseDto.Issue, caseDto.CurrentStatus,
				caseDto.NextSteps, patchCaseModel.CaseAim, caseDto.DeEscalationPoint, caseDto.DirectionOfTravel,
				caseDto.Urn, caseDto.Status);
		}
	}
}