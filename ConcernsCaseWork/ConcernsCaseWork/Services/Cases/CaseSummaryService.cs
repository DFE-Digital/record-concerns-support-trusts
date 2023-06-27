using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Redis.Trusts;
using ConcernsCaseWork.Service.Cases;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases;

public class CaseSummaryService : CachedService, ICaseSummaryService
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
	
	public CaseSummaryService(ICacheProvider cacheProvider, IApiCaseSummaryService caseSummaryService, ITrustCachedService trustCachedService) : base(cacheProvider)
	{
		_caseSummaryService = caseSummaryService;
		_trustCachedService = trustCachedService;
	}

	public async Task<List<ActiveCaseSummaryModel>> GetActiveCaseSummariesByCaseworker(string caseworker)
	{
		var caseSummaries = await _caseSummaryService.GetActiveCaseSummariesByCaseworker(caseworker);
		return await BuildActiveCaseSummaryModel(caseSummaries);
	}

	public async Task<List<ActiveCaseSummaryModel>> GetActiveCaseSummariesForUsersTeam(string caseworker)
	{
		var caseSummaries = await _caseSummaryService.GetActiveCaseSummariesForUsersTeam(caseworker);
		return await BuildActiveCaseSummaryModel(caseSummaries);
	}
	

	public async Task<PagedCaseSummaryModel> GetActiveCaseSummariesByTrust(string trustUkPrn, int page, int count)
	{
		var caseSummaries = await _caseSummaryService.GetActiveCaseSummariesByTrust(trustUkPrn, page, count);
		return await BuildActiveCaseSummaryModel(caseSummaries);
	}
	
	public async Task<List<ActiveCaseSummaryModel>> GetActiveCaseSummariesByTrust(string trustUkPrn)
	{
		var caseSummaries = await _caseSummaryService.GetActiveCaseSummariesByTrust(trustUkPrn);
		return BuildActiveCaseSummaryModel(caseSummaries).Result.ToList();
	}

	public async Task<List<ClosedCaseSummaryModel>> GetClosedCaseSummariesByTrust(string trustUkPrn)
	{
		var caseSummaries = await _caseSummaryService.GetClosedCaseSummariesByTrust(trustUkPrn);
		return BuildClosedCaseSummaryModel(caseSummaries).Result.ToList();
	}
	public async Task<PagedCaseSummaryModel> GetClosedCaseSummariesByTrust(string trustUkPrn, int page, int count)
	{
		var caseSummaries = await _caseSummaryService.GetClosedCaseSummariesByTrust(trustUkPrn, page, count);
		return await BuildClosedCaseSummaryModel(caseSummaries);
	}

	
	public async Task<List<ClosedCaseSummaryModel>> GetClosedCaseSummariesByCaseworker(string caseworker)
	{
		var caseSummaries = await _caseSummaryService.GetClosedCaseSummariesByCaseworker(caseworker);
		return await BuildClosedCaseSummaryModel(caseSummaries);
	}

	
	
	private async Task<List<ActiveCaseSummaryModel>> BuildActiveCaseSummaryModel(IEnumerable<ActiveCaseSummaryDto> caseSummaries)
	{
		IEnumerable<ActiveCaseSummaryDto> activeCaseSummaryDtos = caseSummaries as ActiveCaseSummaryDto[] ?? caseSummaries.ToArray();
		var getTrustNameTasks = activeCaseSummaryDtos.DistinctBy(x => x.TrustUkPrn).Select(x => GetTrust(x.TrustUkPrn));
		var trusts = await Task.WhenAll(getTrustNameTasks);
		List<ActiveCaseSummaryModel> sortedCaseSummaries = BuildActiveCaseSummaryModels(activeCaseSummaryDtos.ToList(), trusts);
		return sortedCaseSummaries.ToList();
	}

	private async Task<PagedCaseSummaryModel> BuildActiveCaseSummaryModel(PagedCasesDto caseSummaries)
	{
		var summary = new PagedCaseSummaryModel()
		{
			HasPrevious = caseSummaries.PageData.HasPrevious,
			NextPageUrl = caseSummaries.PageData.NextPageUrl,
			Page = caseSummaries.PageData.Page,
			RecordCount = caseSummaries.PageData.RecordCount,
			HasNext = caseSummaries.PageData.HasNext,

		};
		if (caseSummaries.ActiveCases.Count > 0)
		{
			var getTrustNameTasks = caseSummaries.ActiveCases.DistinctBy(x => x.TrustUkPrn).Select(x => GetTrust(x.TrustUkPrn));
			var trusts = await Task.WhenAll(getTrustNameTasks);
			summary.ActiveCases = BuildActiveCaseSummaryModels(caseSummaries.ActiveCases, trusts);
			
			
		}
		return summary;
	}

	

private static List<ActiveCaseSummaryModel> BuildActiveCaseSummaryModels(List<ActiveCaseSummaryDto> activeCaseSummaryDtos, KeyValuePair<string, string>[] trusts)
	{
		var sortedCaseSummaries = new List<ActiveCaseSummaryModel>();
		foreach (var caseSummary in activeCaseSummaryDtos.OrderByDescending(cs => cs.CreatedAt))
		{
			var sortedActionAndDecisionNames = GetSortedActionAndDecisionNames(caseSummary);

			var summary =
				new ActiveCaseSummaryModel
				{
					ActiveConcerns = GetSortedActiveConcerns(caseSummary.ActiveConcerns),
					ActiveActionsAndDecisions =
						sortedActionAndDecisionNames.Take(_maxNumberActionsAndDecisionsToReturn).ToArray(),
					CaseUrn = caseSummary.CaseUrn,
					CreatedAt = DateTimeHelper.ParseToDisplayDate(caseSummary.CreatedAt),
					CreatedBy = GetDisplayUserName(caseSummary.CreatedBy),
					IsMoreActionsAndDecisions = sortedActionAndDecisionNames.Length > _maxNumberActionsAndDecisionsToReturn,
					Rating = RatingMapping.MapDtoToModel(caseSummary.Rating),
					StatusName = caseSummary.StatusName,
					TrustName = trusts.Single(x => x.Key == caseSummary.TrustUkPrn).Value,
					UpdatedAt = DateTimeHelper.ParseToDisplayDate(caseSummary.UpdatedAt)
				};

			sortedCaseSummaries.Add(summary);
		}

		return sortedCaseSummaries;
	}
	
	private async Task<PagedCaseSummaryModel> BuildClosedCaseSummaryModel(PagedCasesDto caseSummaries)
	{
		var sortedCaseSummaries = new List<ClosedCaseSummaryModel>();
		var model = new PagedCaseSummaryModel()
		{
			NextPageUrl = caseSummaries.PageData.NextPageUrl,
			Page = caseSummaries.PageData.Page,
			RecordCount = caseSummaries.PageData.RecordCount,
			HasNext = caseSummaries.PageData.HasNext,
			HasPrevious = caseSummaries.PageData.HasPrevious
		};
			
		foreach (var caseSummary in caseSummaries.ClosedCases.OrderByDescending(cs => cs.CreatedAt))
		{
			var trustName = await GetTrustName(caseSummary.TrustUkPrn);
			
			var sortedActionAndDecisionNames = GetSortedActionAndDecisionNames(caseSummary);
			
			var summary = 
				new ClosedCaseSummaryModel
				{
					ClosedConcerns = GetSortedClosedConcerns(caseSummary.ClosedConcerns),
					ClosedActionsAndDecisions = sortedActionAndDecisionNames.Take(_maxNumberActionsAndDecisionsToReturn).ToArray(),
					ClosedAt = DateTimeHelper.ParseToDisplayDate(caseSummary.ClosedAt),
					CaseUrn = caseSummary.CaseUrn,
					CreatedAt = DateTimeHelper.ParseToDisplayDate(caseSummary.CreatedAt),
					CreatedBy = GetDisplayUserName(caseSummary.CreatedBy),
					IsMoreActionsAndDecisions = sortedActionAndDecisionNames.Length > _maxNumberActionsAndDecisionsToReturn,
					StatusName = caseSummary.StatusName,
					TrustName = trustName,
					UpdatedAt = DateTimeHelper.ParseToDisplayDate(caseSummary.UpdatedAt)
				};
			
			sortedCaseSummaries.Add(summary);
		}
		model.ClosedCases = sortedCaseSummaries.ToList();
		return model;
	}

	private async Task<List<ClosedCaseSummaryModel>> BuildClosedCaseSummaryModel(IEnumerable<ClosedCaseSummaryDto> caseSummaries)
	{
		var sortedCaseSummaries = new List<ClosedCaseSummaryModel>();

		foreach (var caseSummary in caseSummaries.OrderByDescending(cs => cs.CreatedAt))
		{
			var trustName = await GetTrustName(caseSummary.TrustUkPrn);
			
			var sortedActionAndDecisionNames = GetSortedActionAndDecisionNames(caseSummary);
			
			var summary = 
				new ClosedCaseSummaryModel
				{
					ClosedConcerns = GetSortedClosedConcerns(caseSummary.ClosedConcerns),
					ClosedActionsAndDecisions = sortedActionAndDecisionNames.Take(_maxNumberActionsAndDecisionsToReturn).ToArray(),
					ClosedAt = DateTimeHelper.ParseToDisplayDate(caseSummary.ClosedAt),
					CaseUrn = caseSummary.CaseUrn,
					CreatedAt = DateTimeHelper.ParseToDisplayDate(caseSummary.CreatedAt),
					CreatedBy = GetDisplayUserName(caseSummary.CreatedBy),
					IsMoreActionsAndDecisions = sortedActionAndDecisionNames.Length > _maxNumberActionsAndDecisionsToReturn,
					StatusName = caseSummary.StatusName,
					TrustName = trustName,
					UpdatedAt = DateTimeHelper.ParseToDisplayDate(caseSummary.UpdatedAt)
				};
			
			sortedCaseSummaries.Add(summary);
		}
		
		return sortedCaseSummaries.ToList();
	}

	private static string[] GetSortedActionAndDecisionNames(CaseSummaryDto caseSummary)
	{
		var allActionsAndDecisions = new List<CaseSummaryDto.ActionDecisionSummaryDto>();
		
		allActionsAndDecisions.AddRange(caseSummary.Decisions);
		allActionsAndDecisions.AddRange(caseSummary.FinancialPlanCases);
		allActionsAndDecisions.AddRange(caseSummary.NoticesToImprove);
		allActionsAndDecisions.AddRange(caseSummary.NtisUnderConsideration);
		allActionsAndDecisions.AddRange(caseSummary.NtiWarningLetters);
		allActionsAndDecisions.AddRange(caseSummary.SrmaCases);
		allActionsAndDecisions.AddRange(caseSummary.TrustFinancialForecasts);

		return allActionsAndDecisions
			.OrderBy(action => action.CreatedAt)
			.Select(action => action.Name)
			.ToArray();
	}
	
	
	private static string[] GetSortedActionAndDecisionNames(ActivePagedCasesDto caseSummary)
	{
		var allActionsAndDecisions = new List<ActivePagedCasesDto.ActionDecisionSummaryDto>();
		
		allActionsAndDecisions.AddRange(caseSummary.Decisions);
		allActionsAndDecisions.AddRange(caseSummary.FinancialPlanCases);
		allActionsAndDecisions.AddRange(caseSummary.NoticesToImprove);
		allActionsAndDecisions.AddRange(caseSummary.NtisUnderConsideration);
		allActionsAndDecisions.AddRange(caseSummary.NtiWarningLetters);
		allActionsAndDecisions.AddRange(caseSummary.SrmaCases);
		allActionsAndDecisions.AddRange(caseSummary.TrustFinancialForecasts);

		return allActionsAndDecisions
			.OrderBy(action => action.CreatedAt)
			.Select(action => action.Name)
			.ToArray();
	}

	private static string[] GetSortedActiveConcerns(IEnumerable<CaseSummaryDto.ConcernSummaryDto> concerns)
	{
		// Worst risk rating first. If risk ratings are the same, oldest first (by create date)
		var result = new List<string>();
		
		foreach (var rating in SortedRags)
		{
			result
				.AddRange(concerns
					.Where(c => c.Rating.Name == rating)
					.OrderBy(r => r.CreatedAt)
					.Select(c => c.Name));

			if (result.Count == concerns.Count())
			{
				break;
			}
		}
		
		result
			.AddRange(concerns
				.Where(c => !SortedRags.Contains(c.Rating.Name))
				.OrderBy(r => r.CreatedAt)
				.Select(c => c.Name));
		
		return result.ToArray();
	}
	
	private static string[] GetSortedClosedConcerns(IEnumerable<CaseSummaryDto.ConcernSummaryDto> concerns)
	{
		// Oldest first (by create date)
		var result = concerns
				.OrderBy(r => r.CreatedAt)
				.Select(c => c.Name);
		
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
	
	private async Task<KeyValuePair<string, string>> GetTrust(string trustUkPrn)
	{
		try
		{
			var trust = await _trustCachedService.GetTrustSummaryByUkPrn(trustUkPrn);
			return trust?.TrustName != null ?
				KeyValuePair.Create(trustUkPrn,  trust.TrustName) :
				KeyValuePair.Create(trustUkPrn, "Unknown");
		}
		catch
		{
			return KeyValuePair.Create(trustUkPrn, "Unknown");
		}
	}

	private static string GetDisplayUserName(string userName)
	{
		if (userName == null)
		{
			return userName;
		}

		return userName.Contains('@') ? userName[..userName.IndexOf('@')].Replace(".", " ").ToTitle() : userName;
	}
}