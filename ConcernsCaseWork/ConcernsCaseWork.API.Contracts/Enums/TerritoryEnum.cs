using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Enums;

public enum TerritoryEnum
{
	[Description("Midlands and West - South West")]
	MidlandsAndWestSouthWest,
	[Description("Midlands and West - West Midlands")]
	MidlandsAndWestWestMidlands,
	[Description("Midlands and West - East Midlands")]
	MidlandsAndWestEastMidlands,
	[Description("North and UTC - North East")]
	NorthAndUtcNorthEast,
	[Description("North and UTC - North West")]
	NorthAndUtcNorthWest,
	[Description("North and UTC - Yorkshire and The Humber")]
	NorthAndUtcYorkshireAndHumber,
	[Description("North and UTC - UTC")]
	NorthAndUtcUtc,
	[Description("South and East - South East")]
	SouthAndSouthEastSouthEast,
	[Description("South and East - London")]
	SouthAndSouthEastLondon,
	[Description("South and East - East of England")]
	SouthAndSouthEastEastOfEngland,
	[Description("National Operations")]
	NationalOperations,
}