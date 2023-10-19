using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.TrustFinancialForecast;

public enum ForecastingToolRanAt
{
	[Description("Current year - Spring")]
	CurrentYearSpring = 1,

	// Description does not match enum
	// We can't change this because the string CurrentYearSummer is the value
	// Only the display name can change
	[Description("Current year - Autumn")]
	CurrentYearSummer = 2,
	[Description("Previous year - Spring")]
	PreviousYearSpring = 3,

	// Description does not match enum
	// We can't change this because the string PreviousYearSummer is the value
	// Only the display name can change
	[Description("Previous year - Autumn")]
	PreviousYearSummer = 4
}
