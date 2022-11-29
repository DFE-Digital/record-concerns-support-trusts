using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Trusts;
using ConcernsCaseWork.Service.Cases;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases;

public class CaseSummaryService : ICaseSummaryService
{	
	private readonly IApiCaseSummaryService _caseSummaryService;
	private readonly ITrustCachedService _trustCachedService;
	private const int _maxNumberActionsAndDecisionsToReturn = 3;
	private static IEnumerable<string> SortedRags
		=> new[]
		{
			"Red-Plus",
			"Red",
			"Red-Amber",
			"Amber",
			"Amber-Green",
			"Green",
			""
		};
	
	public CaseSummaryService(IApiCaseSummaryService caseSummaryService, ITrustCachedService trustCachedService)
	{
		_caseSummaryService = caseSummaryService;
		_trustCachedService = trustCachedService;
	}

	public async Task<List<ActiveCaseSummaryModel>> GetActiveCaseSummariesByCaseworker(string caseworker)
	{
		var caseSummaries = await _caseSummaryService.GetActiveCaseSummariesByCaseworker(caseworker);

		var sortedCaseSummaries = new List<ActiveCaseSummaryModel>();

		foreach (var caseSummary in caseSummaries.OrderByDescending(cs => cs.CreatedAt))
		{
			var trustName = await GetTrustName(caseSummary.TrustUkPrn);
			
			var sortedActionAndDecisionNames = GetSortedActionAndDecisionNames(caseSummary);
			
			var summary = 
				new ActiveCaseSummaryModel
				{
	                ActiveConcerns = GetSortedConcerns(caseSummary.ActiveConcerns),
	                ActiveActionsAndDecisions = sortedActionAndDecisionNames.Take(_maxNumberActionsAndDecisionsToReturn).ToArray(),
	                CaseUrn = caseSummary.CaseUrn,
	                CreatedAt = caseSummary.CreatedAt.ToDayMonthYear(),
	                CreatedBy = caseSummary.CreatedBy,
	                IsMoreActionsAndDecisions = sortedActionAndDecisionNames.Length > _maxNumberActionsAndDecisionsToReturn,
	                Rating = RatingMapping.MapDtoToModel(caseSummary.Rating),
	                StatusName = caseSummary.StatusName,
	                TrustName = trustName,
	                UpdatedAt = caseSummary.UpdatedAt.ToDayMonthYear()
				};
			
			sortedCaseSummaries.Add(summary);
		}
		
		return sortedCaseSummaries.ToList();
	}
	
	public async Task<List<ActiveCaseSummaryModel>> GetActiveCaseSummariesByCaseworkers(IEnumerable<string> caseWorkers)
	{
		var getSummaryTasks = caseWorkers
			.Select(caseworker => _caseSummaryService.GetActiveCaseSummariesByCaseworker(caseworker))
			.ToArray();

		var caseSummaries = (await Task.WhenAll(getSummaryTasks)).SelectMany(t => t);

		var sortedCaseSummaries = new List<ActiveCaseSummaryModel>();
		foreach (var caseSummary in caseSummaries.OrderByDescending(cs => cs.CreatedAt))
		{
			var trustName = await GetTrustName(caseSummary.TrustUkPrn);
			
			var sortedActionAndDecisionNames = GetSortedActionAndDecisionNames(caseSummary);
			
			var summary = 
				new ActiveCaseSummaryModel
				{
					ActiveConcerns = GetSortedConcerns(caseSummary.ActiveConcerns),
					ActiveActionsAndDecisions = sortedActionAndDecisionNames.Take(_maxNumberActionsAndDecisionsToReturn).ToArray(),
					CaseUrn = caseSummary.CaseUrn,
					CreatedAt = caseSummary.CreatedAt.ToDayMonthYear(),
					CreatedBy = GetDisplayUserName(caseSummary.CreatedBy),
					IsMoreActionsAndDecisions = sortedActionAndDecisionNames.Length > _maxNumberActionsAndDecisionsToReturn,
					Rating = RatingMapping.MapDtoToModel(caseSummary.Rating),
					StatusName = caseSummary.StatusName,
					TrustName = trustName,
					UpdatedAt = caseSummary.UpdatedAt.ToDayMonthYear()
				};
			
			sortedCaseSummaries.Add(summary);
		}
		
		return sortedCaseSummaries.ToList();
	}
	
	private static string[] GetSortedActionAndDecisionNames(ActiveCaseSummaryDto activeCaseSummary)
	{
		var allActionsAndDecisions = new List<ActiveCaseSummaryDto.ActionDecisionSummaryDto>();
		
		allActionsAndDecisions.AddRange(activeCaseSummary.Decisions);
		allActionsAndDecisions.AddRange(activeCaseSummary.FinancialPlanCases);
		allActionsAndDecisions.AddRange(activeCaseSummary.NoticesToImprove);
		allActionsAndDecisions.AddRange(activeCaseSummary.NtisUnderConsideration);
		allActionsAndDecisions.AddRange(activeCaseSummary.NtiWarningLetters);
		allActionsAndDecisions.AddRange(activeCaseSummary.SrmaCases);

		return allActionsAndDecisions
			.OrderBy(action => action.CreatedAt)
			.Select(action => action.Name)
			.ToArray();
	}

	private static string[] GetSortedConcerns(IEnumerable<ActiveCaseSummaryDto.ConcernSummaryDto> concerns)
	{
		var result = new List<string>();
		
		foreach (var rating in SortedRags)
		{
			result
				.AddRange(concerns
					.Where(c => c.Rating.Name == rating)
					.OrderByDescending(r => r.CreatedAt)
					.Select(c => c.Name));
		}
		
		result
			.AddRange(concerns
				.Where(c => !SortedRags.Contains(c.Rating.Name))
				.OrderByDescending(r => r.CreatedAt)
				.Select(c => c.Name));
		
		return result.ToArray();
	}

	private async Task<string> GetTrustName(string trustUkPrn)
	{
		if (trustUkPrn == null)
		{
			return "Trust has no UkPrn";
		}
		
		try
		{
			var trust = await _trustCachedService.GetTrustSummaryByUkPrn(trustUkPrn);	
			return trust?.TrustName ?? $"Trust with UkPrn {trustUkPrn} not found";
		}
		catch
		{
			return $"Error getting Trust with UkPrn {trustUkPrn}";
		}
	}

	private static string GetDisplayUserName(string userName) => userName.Substring(0, userName.IndexOf('@'));
}