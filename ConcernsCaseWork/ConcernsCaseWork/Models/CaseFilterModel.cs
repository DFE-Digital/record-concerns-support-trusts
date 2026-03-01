#nullable enable
using ConcernsCaseWork.API.Contracts.Case;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Models;

public class CaseFilters
{
    public const string _selectedRegionsKey = nameof(SelectedRegions);
	public const string _selectedStatusesKey = nameof(SelectedStatuses);

	public string[] SelectedRegions { get; private set; } = [];
	public string[] SelectedStatuses { get; private set; } = [];

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

	public bool IsVisible => SelectedRegionEnums.Length > 0 || SelectedStatusEnums.Length > 0;

    public void PopulateFrom(IEnumerable<KeyValuePair<string, StringValues>> requestQuery)
    {
        var query = new Dictionary<string, StringValues>(requestQuery, StringComparer.OrdinalIgnoreCase);

        // Explicit clear
        if (query.ContainsKey("clear"))
        {
            SelectedRegions = [];
			SelectedStatuses = [];

			return;
        }

		if (query.TryGetValue(_selectedRegionsKey, out var regions) && regions.Count > 0)
		{
			SelectedRegions = regions
			.Select(v => v?.Trim())
			.Where(v => !string.IsNullOrWhiteSpace(v))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray()!;
		}
		else
		{
			SelectedRegions = [];
		}

		if (query.TryGetValue(_selectedStatusesKey, out var statuses) && statuses.Count > 0)
		{
			SelectedStatuses = statuses
			.Select(v => v?.Trim())
			.Where(v => !string.IsNullOrWhiteSpace(v) && !v.Equals("Unknown"))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray()!;
		}
		else
		{
			SelectedStatuses = [];
		}
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
