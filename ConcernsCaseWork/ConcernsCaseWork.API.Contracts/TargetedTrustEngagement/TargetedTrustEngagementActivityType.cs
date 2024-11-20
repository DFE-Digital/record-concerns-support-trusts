using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.TargetedTrustEngagement
{
	public enum TargetedTrustEngagementActivityType
	{
		[Description("Category 1")]
		Category1 = 1,
		[Description("Category 2")]
		Category2 = 2,
		[Description("Category 3")]
		Category3 = 3,
		[Description("Category 4")]
		Category4 = 4,
		[Description("CEOs")]
		CEOs = 5,
		[Description("Leadership")]
		Leadership = 6,
		[Description("CEOs and Leadership")]
		CEOsAndLeadership = 7,
		[Description("Annual summary internal scrutiny reports")]
		AnnualSummaryInternalScrutinyReports = 8,
		[Description("Audit issues")]
		AuditIssues = 9,
		[Description("Management letter issues")]
		ManagementLetterIssues = 10,
		[Description("Regularity issues")]
		RegularityIssues = 11,
		[Description("Priority 1 ")]
		Priority1 = 12,
		[Description("Priority 2")]
		Priority2 = 13,
		[Description("Priority 3")]
		Priority3 = 14,
		[Description("Priority 4")]
		Priority4 = 15,
		[Description("Budget Forecast Return/Accounts Return driven")]
		BudgetForecastReturnAccountsReturnDriven = 16,
		[Description("Executive pay engagement")]
		ExecutivePayEngagement = 17,
		[Description("Financial returns assurance")]
		FinancialReturnsAssurance = 18,
		[Description("Reserves Oversight Assurance Project")]
		ReservesOversightAssuranceProject = 19,
		[Description("Local proactive engagement")]
		LocalProactiveEngagement = 20,
		[Description("Other national processes")]
		OtherNationalProcesses = 21
	}
}
