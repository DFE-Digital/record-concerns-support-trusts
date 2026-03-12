#nullable enable
using ConcernsCaseWork.API.Contracts.Case;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Models;

public class CaseFilters
{
	public const string FilterRegions = nameof(FilterRegions);
	public const string FilterOwners = nameof(FilterOwners);
	public const string FilterTeamLeaders = nameof(FilterTeamLeaders);
	public const string FilterStatuses = nameof(FilterStatuses);

	private IDictionary<string, object?> _filterStore = new Dictionary<string, object?>();

	public const string _selectedRegionsKey = nameof(SelectedRegions);
	public const string _selectedOwnerKey = nameof(SelectedOwners);
	public const string _selectedCaseTeamLeaderKey = nameof(SelectedTeamLeaders);
	public const string _selectedStatusesKey = nameof(SelectedStatuses);

	[BindProperty] public string[] SelectedRegions { get; private set; } = [];
	[BindProperty] public string[] SelectedOwners { get; private set; } = [];
	[BindProperty] public string[] SelectedTeamLeaders { get; private set; } = [];
	[BindProperty] public string[] SelectedStatuses { get; private set; } = [];

	public List<string> CaseOwners { get; private set; } = [];
	public List<string> TeamLeaders { get; private set; } = [];

	public Region[] SelectedRegionEnums =>
		[.. SelectedRegions
            .Select(TryParseAs<Region>)
            .Where(r => r.HasValue)
            .Select(r => r!.Value)
            .Distinct()];

	public CaseStatus[] SelectedStatusEnums =>
		[.. SelectedStatuses
			.Select(TryParseAs<CaseStatus>)
			.Where(r => r.HasValue)
			.Select(r => r!.Value)
			.Distinct()];

	public bool IsVisible => SelectedRegionEnums.Length > 0 || SelectedOwners.Length > 0 || SelectedTeamLeaders.Length > 0 || SelectedStatuses.Length > 0;

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
		SelectedOwners = GetFilters(FilterOwners);
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
			SelectedOwners = [];
			SelectedTeamLeaders = [];
			SelectedStatuses = [];
			return;
		}

		if (query.ContainsKey("remove"))
		{
			SelectedRegions = RemoveValuesFromFilters(FilterRegions, ExtractQueryItems(nameof(SelectedRegions)));
			SelectedOwners = RemoveValuesFromFilters(FilterOwners, ExtractQueryItems(nameof(SelectedOwners)));
			SelectedTeamLeaders = RemoveValuesFromFilters(FilterTeamLeaders, ExtractQueryItems(nameof(SelectedTeamLeaders)));
			SelectedStatuses = RemoveValuesFromFilters(FilterStatuses, ExtractQueryItems(nameof(SelectedStatuses)));
			return;
		}


		SelectedRegions = UpdateAndGetStore(FilterRegions, ExtractQueryItems(nameof(SelectedRegions)));
		SelectedOwners = UpdateAndGetStore(FilterOwners, ExtractQueryItems(nameof(SelectedOwners)));
		SelectedTeamLeaders = UpdateAndGetStore(FilterTeamLeaders, ExtractQueryItems(nameof(SelectedTeamLeaders)));
		SelectedStatuses = UpdateAndGetStore(FilterStatuses, ExtractQueryItems(nameof(SelectedStatuses)));

		
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
	
	private static T? TryParseAs<T>(string? input) where T : struct, Enum
	{
		if (string.IsNullOrWhiteSpace(input)) return null;

		if (int.TryParse(input, out var i) && Enum.IsDefined(typeof(T), i))
				return (T)Enum.ToObject(typeof(T), i);

		if (Enum.TryParse<T>(input, ignoreCase: true, out var r) && Enum.IsDefined(typeof(T), r))
			return r;

		return null;
	}
}
