using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Enums.TrustFinancialForecast;

public enum ForecastingToolRanAt
{
	[Description("Current year - Spring")]
	CurrentYearSpring = 1,
	[Description("Current year - Summer")]
	CurrentYearSummer = 2,
	[Description("Previous year - Spring")]
	PreviousYearSpring = 3,
	[Description("Previous year - Summer")]
	PreviousYearSummer = 4
}