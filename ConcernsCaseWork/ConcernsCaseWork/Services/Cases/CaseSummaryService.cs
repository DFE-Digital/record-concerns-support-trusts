using AutoMapper;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Service.Cases;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases;

public class CaseSummaryService : ICaseSummaryService
{
	private readonly IApiCaseSummaryService _caseSummaryService;
	private readonly IMapper _mapper;

	public CaseSummaryService(IApiCaseSummaryService caseSummaryService, IMapper mapper)
	{
		_caseSummaryService = caseSummaryService;
		_mapper = mapper;
	}

	public async Task<List<ActiveCaseSummaryModel>> GetActiveCaseSummariesByCaseworker(string caseworker)
	{
		var caseSummaries = await _caseSummaryService.GetActiveCaseSummariesByCaseworker(caseworker);
		
		return caseSummaries.Select(caseSummary => new ActiveCaseSummaryModel
		{
			ActiveConcerns = caseSummary.ActiveConcerns,
			ActiveActionsAndDecisions = GetSortedActionAndDecisionNames(caseSummary),
			CaseUrn = caseSummary.CaseUrn,
			CreatedAt = caseSummary.CreatedAt,
			UpdatedAt = caseSummary.UpdatedAt,
			CreatedBy = caseSummary.CreatedBy,
			Rating = RatingMapping.MapDtoToModel(caseSummary.Rating),
			TrustUkPrn = caseSummary.TrustUkPrn,
			StatusName = caseSummary.StatusName
		}).ToList();
	}
	
	private static string[] GetSortedActionAndDecisionNames(ActiveCaseSummaryDto activeCaseSummary)
	{
		var allActionsAndDecisions = new List<Summary>();
		
		allActionsAndDecisions.AddRange(activeCaseSummary.FinancialPlanCases);
		allActionsAndDecisions.AddRange(activeCaseSummary.NoticesToImprove);
		allActionsAndDecisions.AddRange(activeCaseSummary.NtisUnderConsideration);
		allActionsAndDecisions.AddRange(activeCaseSummary.NtiWarningLetters);
		allActionsAndDecisions.AddRange(activeCaseSummary.SrmaCases);

		return allActionsAndDecisions
			.OrderByDescending(action => action.CreatedAt)
			.Select(action => action.Name)
			.ToArray();
	}
}