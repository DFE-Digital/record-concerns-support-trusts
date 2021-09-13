using AutoMapper;
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
		
		public static CaseModel Map(IMapper mapper, CaseDto caseDto, string statusName)
		{
			var caseModel = mapper.Map<CaseModel>(caseDto);
			caseModel.StatusName = statusName;
			return caseModel;
		}
	}
}