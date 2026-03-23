using ConcernsCaseWork.API.Contracts.Case;

namespace ConcernsCaseWork.Data.Helpers;

public static class RegionTerritoryMapping
{
	public static HashSet<Territory> GetTerritoriesForRegions(IReadOnlyList<Region> regions)
	{
		var set = new HashSet<Territory>();
		foreach (var region in regions)
		{
			var territory = MapRegionToTerritory(region);
			if (territory.HasValue)
			{
				set.Add(territory.Value);
			}
		}

		return set;
	}

	private static Territory? MapRegionToTerritory(Region region)
	{
		return region switch
		{
			Region.EastMidlands => Territory.Midlands_And_West__East_Midlands,
			Region.EastOfEngland => Territory.South_And_South_East__East_Of_England,
			Region.London => Territory.South_And_South_East__London,
			Region.NorthEast => Territory.North_And_Utc__North_East,
			Region.NorthWest => Territory.North_And_Utc__North_West,
			Region.SouthEast => Territory.South_And_South_East__South_East,
			Region.SouthWest => Territory.Midlands_And_West__SouthWest,
			Region.WestMidlands => Territory.Midlands_And_West__West_Midlands,
			Region.YorkshireAndTheHumber => Territory.North_And_Utc__Yorkshire_And_Humber,
			_ => null
		};
	}
}
