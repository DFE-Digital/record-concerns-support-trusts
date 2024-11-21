using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.TargetedTrustEngagement
{
	public enum TargetedTrustEngagementActivity
	{
		[Description("Budget Forecast Return/Accounts Return driven")]
		BudgetForecastReturnAccountsReturnDriven = 1,

		[Description("Executive pay engagement")]
		ExecutivePayEngagement = 2,

		[Description("Financial returns assurance")]
		FinancialReturnsAssurance = 3,

		[Description("Reserves Oversight and Assurance Project")]
		ReservesOversightAndAssuranceProject = 4,

		[Description("Local proactive engagement")]
		LocalProactiveEngagement = 5,

		[Description("Other national processes")]
		OtherNationalProcesses = 6,

		[Description("No engagement activities were taken forward")]
		NoEngagementActivitiesWereTakenForward = 7
	}
}