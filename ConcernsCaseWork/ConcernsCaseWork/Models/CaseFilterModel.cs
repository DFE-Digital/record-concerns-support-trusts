#nullable enable
using ConcernsCaseWork.API.Contracts.Case;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Models;

public class CaseFilters
{
	public const string FilterRegions = nameof(FilterRegions);
	public const string FilterCaseOwners = nameof(FilterCaseOwners);
	public const string FilterTeamLeaders = nameof(FilterTeamLeaders);
	public const string FilterStatuses = nameof(FilterStatuses);

	private IDictionary<string, object?> _filterStore = new Dictionary<string, object?>();

	public const string _selectedRegionsKey = nameof(SelectedRegions);
	public const string _selectedCaseOwnerKey = nameof(SelectedCaseOwners);
	public const string _selectedCaseTeamLeaderKey = nameof(SelectedTeamLeaders);
	public const string _selectedStatusesKey = nameof(SelectedStatuses);

	[BindProperty] public string[] SelectedRegions { get; private set; } = [];
	[BindProperty] public string[] SelectedCaseOwners { get; private set; } = [];
	[BindProperty] public string[] SelectedTeamLeaders { get; private set; } = [];
	[BindProperty] public string[] SelectedStatuses { get; private set; } = [];

	public List<string> CaseOwners { get; private set; } = [];
	public List<string> TeamLeaders { get; private set; } = [];

	public Region[] SelectedRegionEnums =>
		[.. SelectedRegions
            .Select(TryParseRegion)
            .Where(r => r.HasValue)
            .Select(r => r!.Value)
            .Distinct()];

	public CaseStatus[] SelectedStatusEnums =>
		[.. SelectedStatuses
			.Select(TryParseStatus)
			.Where(r => r.HasValue)
			.Select(r => r!.Value)
			.Distinct()];

	public bool IsVisible => SelectedRegionEnums.Length > 0 || SelectedCaseOwners.Length > 0 || SelectedTeamLeaders.Length > 0 || SelectedStatuses.Length > 0;

    public void SetCaseOwners(List<string> caseOwners)
    {
		CaseOwners = caseOwners;
	}

	public void SetCaseTeamLeaders(List<string> teamLeaders)
	{
		TeamLeaders = teamLeaders;
	}

	public CaseFilters PersistUsing(IDictionary<string, object?> filterStore)
	{
		_filterStore = filterStore;

		SelectedRegions = GetFilters(FilterRegions);
		SelectedCaseOwners = GetFilters(FilterCaseOwners);
		SelectedTeamLeaders = GetFilters(FilterTeamLeaders);
		SelectedStatuses = GetFilters(FilterStatuses);

		return this;
	}

	private string[] GetFilters(string filterType)
	{
		return _filterStore.TryGetValue(filterType, out var filters) && filters is string[] values
			? values
			: [];
	}

	public void PopulateFrom(IEnumerable<KeyValuePair<string, StringValues>> requestQuery)
    {
        var query = new Dictionary<string, StringValues>(requestQuery, StringComparer.OrdinalIgnoreCase);

        // Explicit clear
        if (query.ContainsKey("clear"))
        {
            SelectedRegions = [];
			SelectedCaseOwners = [];
			SelectedTeamLeaders = [];
			SelectedStatuses = [];

			return;
        }

        if (query.ContainsKey("remove"))
        {
	        SelectedRegions = RemoveValuesFromFilters(FilterRegions, ExtractQueryItems(nameof(SelectedRegions)));
	        SelectedCaseOwners = RemoveValuesFromFilters(FilterCaseOwners, ExtractQueryItems(nameof(SelectedCaseOwners)));
	        SelectedTeamLeaders = RemoveValuesFromFilters(FilterTeamLeaders, ExtractQueryItems(nameof(SelectedTeamLeaders)));
	        SelectedStatuses = RemoveValuesFromFilters(FilterStatuses, ExtractQueryItems(nameof(SelectedStatuses)));
			return;
        }

        if (query.ContainsKey(nameof(SelectedRegions)) || query.ContainsKey(nameof(SelectedCaseOwners)) || query.ContainsKey(nameof(SelectedTeamLeaders)) || query.ContainsKey(nameof(SelectedStatuses)))
        {
	        SelectedRegions = UpdateAndGetStore(FilterRegions, ExtractQueryItems(nameof(SelectedRegions)));
			SelectedCaseOwners = UpdateAndGetStore(FilterCaseOwners, ExtractQueryItems(nameof(SelectedCaseOwners)));
			SelectedTeamLeaders = UpdateAndGetStore(FilterTeamLeaders, ExtractQueryItems(nameof(SelectedTeamLeaders)));
			SelectedStatuses = UpdateAndGetStore(FilterStatuses, ExtractQueryItems(nameof(SelectedStatuses)));
		}

		
		string[] ExtractQueryItems(string key)
		{
			return query.TryGetValue(key, out var value) ? value! : Array.Empty<string>();
		}
	}

	private string[] RemoveValuesFromFilters(string filterType, string[] valuesToRemove)
	{
		var currentFilters = GetFilters(filterType);

		if (valuesToRemove is { Length: > 0 })
		{
			currentFilters = currentFilters.Where(x => !valuesToRemove.Contains(x)).ToArray();
		}

		UpdateAndGetStore(filterType, currentFilters);

		return currentFilters;
	}

	private string[] UpdateAndGetStore(string key, string[]? value)
	{
		if (value is null || value.Length == 0)
		{
			_filterStore.Remove(key);
			return [];
		}

		_filterStore[key] = value;

		return value;
	}

	private static Region? TryParseRegion(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        if (int.TryParse(input, out var i) && Enum.IsDefined(typeof(Region), i))
            return (Region)i;

        if (Enum.TryParse<Region>(input, ignoreCase: true, out var r) && Enum.IsDefined(typeof(Region), r))
            return r;

        return null;
    }

	private static CaseStatus? TryParseStatus(string? input)
	{
		if (string.IsNullOrWhiteSpace(input)) return null;

		if (int.TryParse(input, out var i) && Enum.IsDefined(typeof(CaseStatus), i))
			return (CaseStatus)i;

		if (Enum.TryParse<CaseStatus>(input, ignoreCase: true, out var r) && Enum.IsDefined(typeof(CaseStatus), r))
			return r;

		return null;
	}
}
