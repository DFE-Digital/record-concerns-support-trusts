using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Enums.TrustFinancialForecast;

public enum WasTrustResponseSatisfactory
{
	[Description("Satisfactory")]
	Satisfactory = 1,
	[Description("Not satisfactory")]
	NotSatisfactory = 2,
}