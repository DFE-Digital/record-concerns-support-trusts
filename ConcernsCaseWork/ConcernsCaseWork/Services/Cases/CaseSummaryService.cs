using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Trusts;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.Trusts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases;

public class CaseSummaryService : ICaseSummaryService
{
	private readonly IApiCaseSummaryService _caseSummaryService;
	private readonly ITrustCachedService _trustCachedService;

	public CaseSummaryService(IApiCaseSummaryService caseSummaryService, ITrustCachedService trustCachedService)
	{
		_caseSummaryService = caseSummaryService;
		_trustCachedService = trustCachedService;
	}

	public async Task<List<ActiveCaseSummaryModel>> GetActiveCaseSummariesByCaseworker(string caseworker)
	{
		var caseSummaries = await _caseSummaryService.GetActiveCaseSummariesByCaseworker(caseworker);

		var results = new List<ActiveCaseSummaryModel>();

		foreach (var caseSummary in caseSummaries)
		{
			var trustName = "Trust not found";

			if (caseSummary.TrustUkPrn != null)
			{
				try
				{
					var trust = await _trustCachedService.GetTrustSummaryByUkPrn(caseSummary.TrustUkPrn);	
					if (trust != null)
                    {
                        trustName = trust.TrustName ?? trustName;
                    }
				}
				catch (Exception)
				{
					// Log
				}
			}

			var summary = 
				new ActiveCaseSummaryModel
				{
	                ActiveConcerns = caseSummary.ActiveConcerns,
	                ActiveActionsAndDecisions = GetSortedActionAndDecisionNames(caseSummary),
	                CaseUrn = caseSummary.CaseUrn,
	                CreatedAt = caseSummary.CreatedAt,
	                CreatedAtDisplay = caseSummary.CreatedAt.ToDayMonthYear(),
	                UpdatedAt = caseSummary.UpdatedAt.ToDayMonthYear(),
	                CreatedBy = caseSummary.CreatedBy,
	                Rating = RatingMapping.MapDtoToModel(caseSummary.Rating),
	                TrustName = trustName,
	                StatusName = caseSummary.StatusName
				};
			
			results.Add(summary);
		}
		
		return results;
	}
	
	private static string[] GetSortedActionAndDecisionNames(ActiveCaseSummaryDto activeCaseSummary)
	{
		var allActionsAndDecisions = new List<Summary>();
		
		allActionsAndDecisions.AddRange(activeCaseSummary.FinancialPlanCases);
		allActionsAndDecisions.AddRange(activeCaseSummary.NoticesToImprove);
		allActionsAndDecisions.AddRange(activeCaseSummary.NtisUnderConsideration);
		allActionsAndDecisions.AddRange(activeCaseSummary.NtiWarningLetters);
		allActionsAndDecisions.AddRange(activeCaseSummary.SrmaCases);
		allActionsAndDecisions.AddRange(activeCaseSummary.Decisions);

		return allActionsAndDecisions
			.OrderByDescending(action => action.CreatedAt)
			.Select(action => action.Name)
			.ToArray();
	}
}