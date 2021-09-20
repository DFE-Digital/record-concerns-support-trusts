using ConcernsCaseWork.Models;
using Service.TRAMS.Cases;

namespace ConcernsCaseWork.Mappers
{
	public static class CaseMapping
	{
		public static CreateCaseDto Map(CreateCaseModel createCaseModel)
		{
			return new CreateCaseDto(createCaseModel.CreatedAt, createCaseModel.UpdatedAt, createCaseModel.ReviewAt, createCaseModel.ClosedAt, 
				createCaseModel.CreatedBy, createCaseModel.Description, createCaseModel.CrmEnquiry, createCaseModel.TrustUkPrn, createCaseModel.ReasonAtReview,
				createCaseModel.DeEscalation, createCaseModel.Issue, createCaseModel.CurrentStatus, createCaseModel.NextSteps, createCaseModel.ResolutionStrategy,
				createCaseModel.DirectionOfTravel, createCaseModel.Urn, createCaseModel.Status);
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
				ResolutionStrategy = caseDto.ResolutionStrategy,
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
				caseDto.NextSteps, caseDto.ResolutionStrategy, caseDto.DirectionOfTravel,
				caseDto.Urn, caseDto.Status);
		}
	}
}