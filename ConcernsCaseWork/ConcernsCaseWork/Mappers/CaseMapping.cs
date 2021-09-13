using ConcernsCaseWork.Models;
using Service.TRAMS.Cases;

namespace ConcernsCaseWork.Mappers
{
	public static class CaseMapping
	{
		public static CreateCaseDto Map(CreateCaseModel createCaseModel)
		{
			return new CreateCaseDto(createCaseModel.CreatedAt, createCaseModel.UpdateAt, createCaseModel.ReviewAt, createCaseModel.ClosedAt, 
				createCaseModel.CreatedBy, createCaseModel.Description, string.Empty, createCaseModel.TrustUkPrn, string.Empty,
				createCaseModel.DeEscalation, createCaseModel.Issue, createCaseModel.CurrentStatus, createCaseModel.NextSteps, createCaseModel.ResolutionStrategy,
				string.Empty, createCaseModel.Urn, createCaseModel.Status);
		}
		
		public static CaseModel Map(CaseDto caseDto, string status)
		{
			return new CaseModel(caseDto.CreatedAt, caseDto.UpdatedAt, caseDto.ReviewAt, caseDto.ClosedAt, 
				caseDto.CreatedBy, caseDto.Description, caseDto.CrmEnquiry, caseDto.TrustUkPrn, caseDto.ReasonAtReview,
				caseDto.DeEscalation, caseDto.Issue, caseDto.CurrentStatus, caseDto.NextSteps, caseDto.ResolutionStrategy,
				caseDto.DirectionOfTravel, caseDto.Urn, status);
		}
	}
}