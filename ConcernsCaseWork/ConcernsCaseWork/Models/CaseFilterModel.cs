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

    public string[] SelectedRegions { get; private set; } = Array.Empty<string>();

    public Region[] SelectedRegionEnums =>
        SelectedRegions
            .Select(TryParseRegion)
            .Where(r => r.HasValue)
            .Select(r => r!.Value)
            .Distinct()
            .ToArray();

    public bool IsVisible => SelectedRegionEnums.Length > 0;

    public void PopulateFrom(IEnumerable<KeyValuePair<string, StringValues>> requestQuery)
    {
        var query = new Dictionary<string, StringValues>(requestQuery, StringComparer.OrdinalIgnoreCase);

        // Explicit clear
        if (query.ContainsKey("clear"))
        {
            SelectedRegions = Array.Empty<string>();
            return;
        }

        if (!query.TryGetValue(_selectedRegionsKey, out var values) || values.Count == 0)
        {
            SelectedRegions = Array.Empty<string>();
            return;
        }

        SelectedRegions = values
            .Select(v => v?.Trim())
            .Where(v => string.IsNullOrWhiteSpace(v) is false)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray()!;
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
}
