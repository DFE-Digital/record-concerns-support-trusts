using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Enums;

public enum TerritoryEnum
{
	[Description("Midlands and West - South West")]
	Midlands_And_West__SouthWest,
	[Description("Midlands and West - West Midlands")]
	Midlands_And_West__West_Midlands,
	[Description("Midlands and West - East Midlands")]
	Midlands_And_West__East_Midlands,
	[Description("North and UTC - North East")]
	North_And_Utc__North_East,
	[Description("North and UTC - North West")]
	North_And_Utc__North_West,
	[Description("North and UTC - Yorkshire and The Humber")]
	North_And_Utc__Yorkshire_And_Humber,
	[Description("North and UTC - UTC")]
	North_And_Utc__Utc,
	[Description("South and East - South East")]
	South_And_South_East__South_East,
	[Description("South and East - London")]
	South_And_South_East__London,
	[Description("South and East - East of England")]
	South_And_South_East__East_Of_England,
	[Description("National Operations")]
	National_Operations,
}