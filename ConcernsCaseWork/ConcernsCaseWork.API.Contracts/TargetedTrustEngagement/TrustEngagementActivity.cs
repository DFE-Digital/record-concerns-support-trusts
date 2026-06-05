using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.TargetedTrustEngagement
{
	public enum TrustEngagementActivity
	{
		[Description("Budget Forecast Return/Accounts Return driven")]
		BudgetForecast = 1,
		[Description("Executive pay engagement")]
		ExecutivePayEngagement = 2,
		[Description("Finance returns assurance")]
		FinanceReturnsAssurance = 3,
		[Description("Reserves oversight assurance project")]
		ReservesOversightAssurance = 4,
		[Description("Local proactive engagement")]
		LocalProactiveEngagement = 5,
		[Description("Other vulnerability")]
		OtherVulnerability = 6,
		[Description("No engagement activities were taken forward")]
		NoEngagement = 7
	}

	public enum SubTrustEnagementActivity
	{
		[Description("Category 1")]
		Category1 = 1,
		[Description("Category 2")]
		Category2 = 2,
		[Description("Category 3")]
		Category3 = 3,
		[Description("Category 4")]
		Category4 = 4,
		[Description("CEO's")]
		Ceos = 5,
		[Description("Leadership")]
		Leadership = 6,
		[Description("CEO's and Leadership")]
		CeosAndLeadership = 7,
		[Description("Audit issues")]
		AuditIssues = 8,
		[Description("Regularity issues")]
		RegularityIssues = 9,
		[Description("Management letter issues")]
		ManagementLetterIssues = 10,
		[Description("Annual summary internal scrutiny reports")]
		AnnualSummaryInternalScrutiny = 11,
		[Description("Priority 1")]
		Priority1 = 12,
		[Description("Priority 2")]
		Priority2 = 13,
		[Description("Priority 3")]
		Priority3 = 14,
		[Description("Priority 4")]
		Priority4 = 15,
		BudgetForecastDriven = 16,
		ExecutivePayEngagement = 17,
		FinanceReturnsAssurance = 18,
		ReservesOversightAssurance = 19,
		LocalProactiveEngagement = 20,
		OtherVunerability = 21,
	}

	public static class TrustEngagementActivityExtensions
	{
		public static SubTrustEnagementActivity[] GetSubActivities(this TrustEngagementActivity trustEngagementActivity)
		{
			switch (trustEngagementActivity)
			{
				case TrustEngagementActivity.BudgetForecast:
					return GetBudgetForecastSubActivities();
				case TrustEngagementActivity.ExecutivePayEngagement:
					return GetExecutivePayEngagementSubActivities();
				case TrustEngagementActivity.FinanceReturnsAssurance:
					return GetFinanceReturnsAssuranceSubActivities();
				case TrustEngagementActivity.ReservesOversightAssurance:
					return GetReservesOversightAssuranceSubActivities();
				default:
					return [];
			}
		}

		public static SubTrustEnagementActivity[] GetBudgetForecastSubActivities()
		{
			return
			[
				SubTrustEnagementActivity.Category1,
				SubTrustEnagementActivity.Category2,
				SubTrustEnagementActivity.Category3,
				SubTrustEnagementActivity.Category4,
			];
		}

		public static SubTrustEnagementActivity[] GetExecutivePayEngagementSubActivities()
		{
			return
			[
				SubTrustEnagementActivity.Ceos,
				SubTrustEnagementActivity.Leadership,
				SubTrustEnagementActivity.CeosAndLeadership,
			];
		}

		public static SubTrustEnagementActivity[] GetFinanceReturnsAssuranceSubActivities()
		{
			return
			[
				SubTrustEnagementActivity.AuditIssues,
				SubTrustEnagementActivity.RegularityIssues,
				SubTrustEnagementActivity.ManagementLetterIssues,
				SubTrustEnagementActivity.AnnualSummaryInternalScrutiny,
			];
		}

		public static SubTrustEnagementActivity[] GetReservesOversightAssuranceSubActivities()
		{
			return
			[
				SubTrustEnagementActivity.Priority1,
				SubTrustEnagementActivity.Priority2,
				SubTrustEnagementActivity.Priority3,
				SubTrustEnagementActivity.Priority4,
			];
		}

		public static SubTrustEnagementActivity[] GetOtherEngagementSubActivities()
		{
			return
			[
				SubTrustEnagementActivity.BudgetForecastDriven,
				SubTrustEnagementActivity.ExecutivePayEngagement,
				SubTrustEnagementActivity.FinanceReturnsAssurance,
				SubTrustEnagementActivity.ReservesOversightAssurance,
				SubTrustEnagementActivity.LocalProactiveEngagement,
				SubTrustEnagementActivity.OtherVunerability,
			];
		}
	}
}
