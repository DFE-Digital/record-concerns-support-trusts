using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Case;

public enum Territory
{
	[Description("South - South West")]
	Midlands_And_West__SouthWest,
	[Description("North - West Midlands")]
	Midlands_And_West__West_Midlands,
	[Description("North - East Midlands")]
	Midlands_And_West__East_Midlands,
	[Description("North - North East")]
	North_And_Utc__North_East,
	[Description("North - North West")]
	North_And_Utc__North_West,
	[Description("North - Yorkshire and The Humber")]
	North_And_Utc__Yorkshire_And_Humber,
	[Description("North and UTC - UTC")]
	North_And_Utc__Utc,
	[Description("South - South East")]
	South_And_South_East__South_East,
	[Description("South - London")]
	South_And_South_East__London,
	[Description("South - East of England")]
	South_And_South_East__East_Of_England,
	[Description("National Operations")]
	National_Operations,
}